namespace aemarco.Crawler.Wallpaper.Common;

internal abstract class WallpaperCrawlerBasis
{

    internal event EventHandler? EntryFound;

    private readonly int _startPage;
    private readonly int _lastPage;
    private readonly bool _onlyNews;
    internal WallpaperCrawlerBasis(
        int startPage,
        int lastPage,
        bool onlyNews)
    {
        _startPage = startPage;
        _lastPage = lastPage;
        _onlyNews = onlyNews;
    }

    #region Starting

    // ReSharper disable once InconsistentNaming
    internal List<CrawlOffer>? _crawlOffers;
    /// <summary>
    /// null or empty list means no limitation, otherwise this crawler will be 
    /// limited to offers which matching searchedCategories
    /// </summary>
    /// <param name="searchedCategories">strings of internal Category (enum)</param>
    internal void LimitAsPerFilterList(List<string> searchedCategories)
    {
        _crawlOffers ??= GetCrawlsOffers();
        _crawlOffers = _crawlOffers
            .Where(o =>
                searchedCategories.Count == 0 || // no filtering = allOffers
                searchedCategories.Contains(o.Category.Category)) //filtering means offers category must be contained
            .ToList();
    }

    internal IEnumerable<CrawlOffer> GetOffers()
    {
        try
        {
            _crawlOffers ??= GetCrawlsOffers();
            return _crawlOffers;
        }
        catch
        {
            // ignored
        }
        return [];
    }


    /// <summary>
    /// Maps Site Category name to internal category
    /// </summary>
    /// <param name="categoryName">name of category on site</param>
    /// <returns>internal category</returns>
    protected virtual ContentCategory? GetContentCategory(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return null;

        return categoryName switch
        {
            "Erotic Wallpapers" => new ContentCategory(Category.Girls),
            "Erotic Girls" => new ContentCategory(Category.Girls),
            "Celebrities" => new ContentCategory(Category.Girls_Celebrities),
            "Celebrity Fakes" => new ContentCategory(Category.Girls_CelebrityFakes),
            "Girls & Cars" => new ContentCategory(Category.Girls_Cars),
            "Girls & Bikes" => new ContentCategory(Category.Girls_Bikes),
            "______Girls_Guns" => new ContentCategory(Category.Girls_Guns),
            "Anime" => new ContentCategory(Category.Girls_Fantasy),
            "Fantasy Girls" => new ContentCategory(Category.Girls_Fantasy),
            "3D & Vector Girls" => new ContentCategory(Category.Girls_Fantasy),
            "Cosplay" => new ContentCategory(Category.Girls_Cosplay),
            "Lingerie" => new ContentCategory(Category.Girls_Lingerie),
            "Lingerie Models" => new ContentCategory(Category.Girls_Lingerie),
            "Lingerie Girls" => new ContentCategory(Category.Girls_Lingerie),
            "lingerie" => new ContentCategory(Category.Girls_Lingerie),
            "underwear" => new ContentCategory(Category.Girls_Lingerie),
            "stockings" => new ContentCategory(Category.Girls_Lingerie),
            "Ligerie beauty models" => new ContentCategory(Category.Girls_Lingerie),
            "Lesbian" => new ContentCategory(Category.Girls_Lesbians),
            "Lesbians" => new ContentCategory(Category.Girls_Lesbians),
            "Beach" => new ContentCategory(Category.Girls_Beaches),
            "Girls & Beaches" => new ContentCategory(Category.Girls_Beaches),
            "Asian Girls" => new ContentCategory(Category.Girls_Asian),
            "asian" => new ContentCategory(Category.Girls_Asian),
            "Asian" => new ContentCategory(Category.Girls_Asian),
            "Indian" => new ContentCategory(Category.Girls_Asian),
            "Japanese" => new ContentCategory(Category.Girls_Asian),
            "Thai" => new ContentCategory(Category.Girls_Asian),
            "Holidays" => new ContentCategory(Category.Girls_Holidays),
            "Christmas" => new ContentCategory(Category.Girls_Holidays),
            "Fetish Girls" => new ContentCategory(Category.Girls_Fetish),
            "Bondage" => new ContentCategory(Category.Girls_Fetish),
            "Blindfold" => new ContentCategory(Category.Girls_Fetish),
            "Latex" => new ContentCategory(Category.Girls_Fetish),
            "BDSM" => new ContentCategory(Category.Girls_Fetish),
            "______Girls_Blowjob" => new ContentCategory(Category.Girls_Blowjob),
            "Hardcore" => new ContentCategory(Category.Girls_Hardcore),
            "Homemade" => new ContentCategory(Category.Girls_Amateur),
            "Amateur" => new ContentCategory(Category.Girls_Amateur),
            "Gloryhole" => new ContentCategory(Category.Girls_Gloryhole),
            "Selfie" => new ContentCategory(Category.Girls_Selfies),
            "Selfpics" => new ContentCategory(Category.Girls_Selfies),
            "Self Shot" => new ContentCategory(Category.Girls_Selfies),
            "______Girls_Fun" => new ContentCategory(Category.Girls_Fun),

            //some to be ignored
            "Arab" => null,
            "BBW" => null,
            "CFNM" => null,
            "Cougar" => null,
            "Femdom" => null,
            "Granny" => null,
            "Housewife" => null,
            "Ladyboy" => null,
            "Mature" => null,
            "MILF" => null,
            "Mom" => null,
            "Party" => null,
            "Pegging" => null,
            "Saggy Tits" => null,
            "Shemale" => null,
            "SSBBW" => null,
            "Wife" => null,
            "Chubby" => null,
            "Gay" => null,
            _ => DefaultCategory
        };
    }

    protected virtual ContentCategory? DefaultCategory => null;

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
            _startPage,
            _lastPage,
            _onlyNews,
            siteCategoryName,
            categoryUri,
            category);
    }


    private List<string> _knownUrls = [];
    protected CancellationToken CancellationToken;




    /// <summary>
    /// returns on completion, false means aborted
    /// </summary>
    /// <returns></returns>
    public async Task<WallCrawlerResult> Start(
        List<string> knownUrls,
        CancellationToken cancellationToken)
    {
        _knownUrls = knownUrls;
        CancellationToken = cancellationToken;

        await Task.Run(() =>
        {
            HandleAll();
            CancellationToken.ThrowIfCancellationRequested();
        }, cancellationToken);

        return _result;
    }


    #endregion

    #region Crawling

    /// <summary>
    /// handles the entire crawler by going through offers
    /// </summary>
    private void HandleAll()
    {
        _crawlOffers ??= GetCrawlsOffers();
        foreach (var offer in _crawlOffers)
        {
            CancellationToken.ThrowIfCancellationRequested();
            HandleOffer(offer);
        }
    }
    protected abstract List<CrawlOffer> GetCrawlsOffers();

    /// <summary>
    /// handles an entire offer by going through pages
    /// </summary>
    /// <param name="catJob">CrawlOffer</param>
    protected virtual void HandleOffer(CrawlOffer catJob)
    {
        do
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (!HandlePage(catJob))
                catJob.ReportEndReached();
        }
        while (catJob.ContainsPagesToDo);
    }


    private int _entriesPerPage;
    /// <summary>
    /// handles current page of given offer (crawlers may override, with API for example)
    /// </summary>
    /// <param name="catJob">job to get page for</param>
    /// <returns>true if page contained any entries</returns>
    protected virtual bool HandlePage(CrawlOffer catJob)
    {
        //site with wallpaper list
        var pageUri = GetSiteUrlForCategory(catJob);
        var pageDoc = pageUri.Navigate();
        var pageNodes = pageDoc.FindNodes(GetSearchStringGorEntryNodes());

        //non entries on page
        if (pageNodes.Count == 0)
            return false;


        //report count to all classic jobs
        if (pageNodes.Count > _entriesPerPage)
        {
            _entriesPerPage = pageNodes.Count;
            _crawlOffers!.ForEach(j =>
            {
                if (j.CrawlMethod == CrawlMethod.Classic)
                    j.ReportNumberOfEntriesPerPage(_entriesPerPage);
            });
        }


        var result = false;
        //handle each node
        foreach (var node in pageNodes)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (catJob.ReachedMaximumStreak)
            {
                catJob.ReportEndReached();
                return true;
            }
            if (AddWallEntry(node, catJob))
            {
                result = true;
            }
        }
        catJob.ReportPageDone();
        //valid Page contains minimum 1 valid Entry
        return result;
    }

    #endregion

    //to be implemented

    /// <summary>
    /// Gets the url for the site containing the category
    /// </summary>
    /// <param name="catJob">current job</param>
    /// <returns>Uri to navigate for category</returns>
    protected abstract PageUri GetSiteUrlForCategory(CrawlOffer catJob);
    /// <summary>
    /// Gets search path for entry nodes
    /// </summary>
    /// <returns>search path</returns>
    protected abstract string GetSearchStringGorEntryNodes();
    /// <summary>
    /// handles entry in a page
    /// </summary>
    /// <param name="pageNode">Html node for entry</param>
    /// <param name="catJob">current job</param>
    /// <returns>true if found a valid entry</returns>
    protected abstract bool AddWallEntry(PageNode pageNode, CrawlOffer catJob);

    //to be implemented END





    // ReSharper disable once InconsistentNaming
    internal WallCrawlerResult _result = new();



    /// <summary>
    /// adds the wall-entry to the crawl result, as new or known wall
    /// </summary>
    /// <param name="entry">entry to add</param>
    /// <param name="catJob">job leaded to entry</param>
    protected void AddEntry(WallEntry entry, CrawlOffer catJob)
    {
        //new Entry
        if (!_knownUrls.Contains(entry.Url))
        {
            _result.NewEntries.Add(entry);
            catJob.ReportEntryDone(false);
            _knownUrls.Add(entry.Url);
        }
        //known Entry
        else
        {
            _result.KnownEntries.Add(entry);
            catJob.ReportEntryDone(true);
        }

        EntryFound?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// adds the album-entry to the crawlresult, as new or known album
    /// </summary>
    /// <param name="entry">entry to add</param>
    /// <param name="catJob">job leaded to entry</param>
    protected void AddAlbum(AlbumEntry entry, CrawlOffer catJob)
    {

        entry.HasNewEntries = entry.Entries.Any(x => !_knownUrls.Contains(x.Url));

        //known Album
        if (!entry.HasNewEntries)
        {
            _result.KnownAlbums.Add(entry);
            catJob.ReportEntryDone(true);
        }
        //new Album
        else
        {
            _result.NewAlbums.Add(entry);
            catJob.ReportEntryDone(false);
            _knownUrls.AddRange(entry.Entries.Select(x => x.Url));
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