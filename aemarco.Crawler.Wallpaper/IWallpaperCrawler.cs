namespace aemarco.Crawler.Wallpaper;

public interface IWallpaperCrawler
{
    void Configure(Func<List<string>>? knownUrlsFunc = null);

    //filter sites
    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    IEnumerable<string> AvailableSourceSites { get; }
    /// <summary>
    /// Used to filter which sites to limit to
    /// Not using means all sites will be crawled
    /// Using means only sites added will be crawled
    /// use GetAvailableSourceSites() to know, which are available
    /// </summary>
    /// <param name="sourceSite"></param>
    void AddSourceSiteFilter(string sourceSite);

    //filter categories
    /// <summary>
    /// Gets the available categories for filtering wallpapers.
    /// </summary>
    IEnumerable<string> AvailableCategories { get; }
    /// <summary>
    /// Used to filter which results being of interest to crawl
    /// Not using this means everything will be crawled
    /// Using this means only added categories will be crawled
    /// </summary>
    void AddCategoryFilter(string category);

    /// <summary>
    /// Do the crawling :)
    /// events may be used for data and completion
    /// </summary>
    Task<WallCrawlerResult> CrawlWallpapers(int? startPage = null, int? lastPage = null, CancellationToken cancellationToken = default);

    [Obsolete("Use CrawlWallpapers instead, this will be removed in a future version")]
    Task<WallCrawlerResult> CrawlWallpapers(CancellationToken cancellationToken = default);

}