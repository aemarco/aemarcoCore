namespace aemarco.Crawler.Wallpaper.Common;

internal abstract class WallpaperCrawlerBasis
{

    protected ILogger Logger { get; }

    internal event EventHandler? EntryFound;
    protected WallpaperCrawlerBasis(
        ILogger logger)
    {
        Logger = logger;
    }

    #region Starting

    // ReSharper disable once InconsistentNaming
    internal List<CrawlOffer>? _crawlOffers;
    /// <summary>
    /// null or empty list means no limitation, otherwise this crawler will be
    /// limited to offers which matching searchedCategories
    /// </summary>
    /// <param name="searchedCategories">strings of internal Category (enum)</param>
    internal async Task LimitAsPerFilterList(List<string> searchedCategories)
    {
        _crawlOffers ??= await GetCrawlOffers();
        _crawlOffers = _crawlOffers
            .Where(o =>
                searchedCategories.Count == 0 || // no filtering = allOffers
                searchedCategories.Contains(o.Category.Category)) //filtering means offers category must be contained
            .ToList();
    }

    internal async Task<IReadOnlyList<CrawlOffer>> GetOffers()
    {
        try
        {
            _crawlOffers ??= await GetCrawlOffers();
            return _crawlOffers;
        }
        catch (Exception ex)
        {
            //no offers means the crawler is left out, which must not go unnoticed
            Logger.LogError(ex, "Crawler {crawlerType} failed to get its offers", GetType().Name);
        }
        return [];
    }


    /// <summary>
    /// creates a CrawlOffer which carries the limitation infos
    /// </summary>
    /// <param name="siteCategoryName">Name of the Category on the site</param>
    /// <param name="categoryUri">Uri to navigate to the Category´s page</param>
    /// <param name="category">mapped internal category</param>
    /// <returns>CrawlOffer</returns>
    protected CrawlOffer CreateCrawlOffer(string siteCategoryName, PageUri categoryUri, ContentCategory category)
    {
        return new CrawlOffer(
            siteCategoryName,
            categoryUri,
            category);
    }

    /// <summary>
    /// crawlers which only ever look at the first page of an offer, regardless of the requested paging
    /// </summary>
    protected virtual bool FirstPageOnly => false;

    /// <summary>
    /// crawlers which need configuration (an api key for example) are left out while it is missing
    /// </summary>
    internal virtual bool IsAvailable => true;


    //the consumer's known urls, one snapshot shared by all crawlers of a crawl (read only, so no locking)
    private IReadOnlySet<string> _knownUrls = new HashSet<string>();
    //what the finished offers of this crawler found, so a repeat in a later offer counts as known
    private readonly HashSet<string> _foundUrls = new(StringComparer.Ordinal);
    protected CancellationToken CancellationToken;




    /// <summary>
    /// returns on completion, false means aborted
    /// </summary>
    /// <returns></returns>
    public async Task<WallCrawlerResult> Start(
        IReadOnlySet<string> knownUrls,
        int startPage,
        int lastPage,
        bool onlyNews,
        CancellationToken cancellationToken)
    {
        _knownUrls = knownUrls;
        CancellationToken = cancellationToken;

        if (FirstPageOnly)
        {
            startPage = 1;
            lastPage = 1;
        }

        await HandleAll(startPage, lastPage, onlyNews);
        CancellationToken.ThrowIfCancellationRequested();

        Logger.LogDebug(
            "Crawler {crawlerType} done with {newEntries} new, {knownEntries} known entries, {newAlbums} new, {knownAlbums} known albums and {warnings} warnings",
            GetType().Name,
            _result.NewEntries.Count,
            _result.KnownEntries.Count,
            _result.NewAlbums.Count,
            _result.KnownAlbums.Count,
            _result.Warnings.Count);
        return _result;
    }


    #endregion

    #region Crawling

    /// <summary>
    /// handles the entire crawler by going through offers
    /// </summary>
    private async Task HandleAll(int startPage, int lastPage, bool onlyNews)
    {
        _crawlOffers ??= await GetCrawlOffers();
        //all offers first, HandlePage reports the entries per page to every offer
        foreach (var offer in _crawlOffers)
        {
            offer.SetPaging(startPage, lastPage, onlyNews);
        }
        foreach (var offer in _crawlOffers)
        {
            CancellationToken.ThrowIfCancellationRequested();
            await RunOffer(offer);
        }
    }

    /// <summary>
    /// the offer is the unit: what it found joins the result only when it went through completely;
    /// a failure drops all of it (and the urls it found), otherwise the entries of the pages before the failure
    /// would be known at the next crawl, whose streak of known entries would end it before the missed pages
    /// </summary>
    private async Task RunOffer(CrawlOffer offer)
    {
        try
        {
            await HandleOffer(offer);
        }
        catch (OperationCanceledException) when (CancellationToken.IsCancellationRequested)
        {
            //the caller cancelled, the crawl delivers nothing
            throw;
        }
        catch (Exception ex)
        {
            var discarded = offer.Discard();
            Logger.LogError(ex, "Offer {offer} of {crawlerType} failed on page {page}, its {discarded} entries are dropped",
                offer.SiteCategoryName, GetType().Name, offer.CurrentPage, discarded);
            AddWarning(offer.CategoryUri, $"Offer {offer.SiteCategoryName} failed on page {offer.CurrentPage}, its entries are dropped: {ex.Message}");
            return;
        }

        Logger.LogDebug(
            "Offer {offer} of {crawlerType} done with {newEntries} new, {knownEntries} known entries, {newAlbums} new, {knownAlbums} known albums",
            offer.SiteCategoryName,
            GetType().Name,
            offer.NewEntries.Count,
            offer.KnownEntries.Count,
            offer.NewAlbums.Count,
            offer.KnownAlbums.Count);
        _result.NewEntries.AddRange(offer.NewEntries);
        _result.KnownEntries.AddRange(offer.KnownEntries);
        _result.NewAlbums.AddRange(offer.NewAlbums);
        _result.KnownAlbums.AddRange(offer.KnownAlbums);
        _foundUrls.UnionWith(offer.FoundUrls);
    }
    protected abstract Task<List<CrawlOffer>> GetCrawlOffers();

    /// <summary>
    /// handles an entire offer by going through pages
    /// </summary>
    /// <param name="catJob">CrawlOffer</param>
    protected virtual async Task HandleOffer(CrawlOffer catJob)
    {
        do
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (!await HandlePage(catJob))
                catJob.ReportEndReached();
        }
        while (catJob.ContainsPagesToDo);
    }


    /// <summary>
    /// handles current page of given offer
    /// </summary>
    /// <param name="catJob">job to get page for</param>
    /// <returns>true if page contained any entries</returns>
    protected abstract Task<bool> HandlePage(CrawlOffer catJob);

    #endregion





    // ReSharper disable once InconsistentNaming
    internal readonly WallCrawlerResult _result = new();


    /// <summary>
    /// lets a crawler skip expensive work (detail calls) for walls which are known already
    /// </summary>
    protected bool IsKnown(string url, CrawlOffer catJob) =>
        _knownUrls.Contains(url) ||
        _foundUrls.Contains(url) ||
        catJob.FoundUrls.Contains(url);

    /// <summary>
    /// adds the wall-entry to the offer, as new or known wall
    /// </summary>
    /// <param name="entry">entry to add</param>
    /// <param name="catJob">job leaded to entry</param>
    protected void AddEntry(WallEntry entry, CrawlOffer catJob)
    {
        //new Entry
        if (!IsKnown(entry.Url, catJob))
        {
            catJob.NewEntries.Add(entry);
            catJob.ReportEntryDone(false);
            catJob.FoundUrls.Add(entry.Url);
        }
        //known Entry
        else
        {
            catJob.KnownEntries.Add(entry);
            catJob.ReportEntryDone(true);
        }

        EntryFound?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// adds the album-entry to the offer, as new or known album
    /// </summary>
    /// <param name="entry">entry to add</param>
    /// <param name="catJob">job leaded to entry</param>
    protected void AddAlbum(AlbumEntry entry, CrawlOffer catJob)
    {

        entry.HasNewEntries = entry.Entries.Any(x => !IsKnown(x.Url, catJob));

        //known Album
        if (!entry.HasNewEntries)
        {
            catJob.KnownAlbums.Add(entry);
            catJob.ReportEntryDone(true);
        }
        //new Album
        else
        {
            catJob.NewAlbums.Add(entry);
            catJob.ReportEntryDone(false);
            catJob.FoundUrls.UnionWith(entry.Entries.Select(x => x.Url));
        }
        EntryFound?.Invoke(this, EventArgs.Empty);
    }

    protected void AddWarning(string message)
    {
        _result.Warnings.Add(new Warning(
            CrawlerInfo.FromCrawlerType(GetType()),
            "unknown",
            message));
    }
    protected void AddWarning(PageUri pageUri, string message, string? additionalContext = null)
    {
        additionalContext ??= pageUri switch
        {
            PageNode pageNode => pageNode.Node.InnerHtml,
            PageDocument pageDoc => pageDoc.Document.DocumentNode.InnerHtml,
            _ => null
        };
        _result.Warnings.Add(new Warning(
            CrawlerInfo.FromCrawlerType(GetType()),
            pageUri.Uri.AbsoluteUri,
            message,
            additionalContext));
    }
}