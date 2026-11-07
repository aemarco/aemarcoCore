namespace aemarco.Crawler.Wallpaper.Common;

/// <summary>
/// crawlers of sites which are read from their html pages: a list page per offer page, a node per entry
/// </summary>
internal abstract class HtmlCrawlerBasis : WallpaperCrawlerBasis
{

    private readonly IHtmlClient _htmlClient;
    protected HtmlCrawlerBasis(
        IHtmlClient htmlClient,
        ILogger logger)
        : base(logger)
    {
        _htmlClient = htmlClient;
    }

    /// <summary>
    /// loads a page through the crawler rate limiting; null in, null out
    /// </summary>
    protected async Task<PageDocument?> Navigate(PageUri? pageUri) =>
        await _htmlClient.Navigate(pageUri, CancellationToken);

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
            "Blowjob" => new ContentCategory(Category.Girls_Blowjob, 90, 94),
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

    private int _entriesPerPage;
    /// <summary>
    /// handles current page of given offer
    /// </summary>
    /// <param name="catJob">job to get page for</param>
    /// <returns>true if page contained any entries</returns>
    protected override async Task<bool> HandlePage(CrawlOffer catJob)
    {
        //site with wallpaper list
        if (await Navigate(GetSiteUrlForCategory(catJob)) is not { } pageDoc)
            return false;
        var pageNodes = pageDoc.FindNodes(GetSearchStringForEntryNodes());

        //non entries on page
        if (pageNodes.Count == 0)
            return false;


        //report count to all offers, they share the layout
        if (pageNodes.Count > _entriesPerPage)
        {
            _entriesPerPage = pageNodes.Count;
            _crawlOffers!.ForEach(j => j.ReportNumberOfEntriesPerPage(_entriesPerPage));
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
            if (await AddWallEntry(node, catJob))
            {
                result = true;
            }
        }
        catJob.ReportPageDone();
        //valid Page contains minimum 1 valid Entry
        return result;
    }

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
    protected abstract string GetSearchStringForEntryNodes();
    /// <summary>
    /// handles entry in a page
    /// </summary>
    /// <param name="pageNode">Html node for entry</param>
    /// <param name="catJob">current job</param>
    /// <returns>true if found a valid entry</returns>
    protected abstract Task<bool> AddWallEntry(PageNode pageNode, CrawlOffer catJob);

    //to be implemented END

}
