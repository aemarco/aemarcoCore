namespace aemarco.Crawler.Wallpaper;

/// <summary>
/// configured through AddWallCrawler(...)
/// </summary>
public class WallCrawlerSettings
{
    /// <summary>
    /// api key of a wallhaven.cc account, the Wallhaven crawler is only available with it
    /// </summary>
    public string? WallhavenApiKey { get; set; }
}
