namespace aemarco.Crawler.Person.Common;

internal interface ISiteCrawlerProvider
{
    string[] GetAvailableCrawlerNames();
    ISiteCrawler[] GetFilteredCrawlerInstances(string[] filter);
}

internal class SiteCrawlerProvider : ISiteCrawlerProvider
{

    private readonly Dictionary<string, CrawlerAttribute> _crawlerInfos;
    private readonly IServiceProvider _serviceProvider;
    public SiteCrawlerProvider(
        Dictionary<string, CrawlerAttribute> crawlerInfos,
        IServiceProvider serviceProvider)
    {
        _crawlerInfos = crawlerInfos;
        _serviceProvider = serviceProvider;
    }

    public string[] GetAvailableCrawlerNames()
    {
        var result = _crawlerInfos.Values
            .OrderBy(x => x.Priority)
            .Select(x => x.FriendlyName)
            .ToArray();
        return result;
    }

    public ISiteCrawler[] GetFilteredCrawlerInstances(string[] filter)
    {
        var result = _crawlerInfos
            .Where(x =>
                filter.Length == 0 ||
                filter.Contains(x.Value.FriendlyName))
            .Select(x => _serviceProvider.GetRequiredKeyedService<ISiteCrawler>(x.Key))
            .ToArray();
        return result;
    }
}