namespace aemarco.Crawler.Person.Common;

internal static class Extensions
{
    internal static PersonCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
    {
        var result = crawlerType.GetCustomAttribute<PersonCrawlerAttribute>();
        return result ?? throw new Exception($"PersonCrawler not defined on {crawlerType.FullName}");
    }


    internal static bool IsAvailableCrawler(this Type crawlerType)
    {
        if (!crawlerType.IsSubclassOf(typeof(PersonCrawlerBase)))
            return false;

        if (crawlerType.Namespace is null || crawlerType.Namespace.EndsWith("Obsolete"))
            return false;


        return true;
    }
}