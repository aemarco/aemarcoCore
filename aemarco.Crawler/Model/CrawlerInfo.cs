namespace aemarco.Crawler.Model;

public record CrawlerInfo(string FriendlyName, int Priority, bool SkipTesting = false)
{
    public static CrawlerInfo FromCrawlerType(Type type)
    {
        var attr = type.GetCustomAttribute<CrawlerAttribute>();
        return attr is null
            ? throw new Exception($"CrawlerInfo not defined on {type.FullName}")
            : new CrawlerInfo(attr.FriendlyName, attr.Priority, attr.SkipTesting);
    }

    public override string ToString() => FriendlyName;


}