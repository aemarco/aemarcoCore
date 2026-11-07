namespace aemarco.Crawler.Wallpaper.Common;

internal interface IWallCrawlerProvider
{
    WallpaperCrawlerBasis[] GetCrawlerInstances();
}

/// <summary>
/// keys of the registered wall crawlers, in scan order
/// (own type, so it does not collide with registrations of the person crawler in the same container)
/// </summary>
internal record WallCrawlerRegistry(string[] CrawlerKeys);

internal class WallCrawlerProvider : IWallCrawlerProvider
{

    private readonly WallCrawlerRegistry _registry;
    private readonly IServiceProvider _serviceProvider;
    public WallCrawlerProvider(
        WallCrawlerRegistry registry,
        IServiceProvider serviceProvider)
    {
        _registry = registry;
        _serviceProvider = serviceProvider;
    }

    //crawlers hold the state of one crawl, so each call delivers fresh instances
    //unavailable ones (missing configuration) are left out, as if they did not exist
    public WallpaperCrawlerBasis[] GetCrawlerInstances()
    {
        var result = _registry.CrawlerKeys
            .Select(x => _serviceProvider.GetRequiredKeyedService<WallpaperCrawlerBasis>(x))
            .Where(x => x.IsAvailable)
            .ToArray();
        return result;
    }
}
