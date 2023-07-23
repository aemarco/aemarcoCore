namespace aemarco.Crawler.Person.Model;

public record CrawlerInfo(string FriendlyName, int Priority)
{
    public static CrawlerInfo FromCrawlerType(Type type)
    {
        var attr = type.GetCustomAttribute<PersonCrawlerAttribute>();
        return attr is null
            ? throw new Exception($"CrawlerInfo not defined on {type.FullName}")
            : new CrawlerInfo(attr.FriendlyName, attr.Priority);
    }
}