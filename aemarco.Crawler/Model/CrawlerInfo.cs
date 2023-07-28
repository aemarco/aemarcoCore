namespace aemarco.Crawler.Model;

public record CrawlerInfo(string FriendlyName, int Priority, bool IsAvailable)
{
    public static CrawlerInfo FromCrawlerType(Type type)
    {
        var obsolete = type.GetCustomAttribute<ObsoleteAttribute>() is not null;
        var attr = type.GetCustomAttribute<CrawlerAttribute>();
        return attr is null
            ? throw new Exception($"CrawlerInfo not defined on {type.FullName}")
            : new CrawlerInfo(attr.FriendlyName, attr.Priority, !obsolete);
    }

    public override string ToString() => FriendlyName;
}