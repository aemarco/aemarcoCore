namespace aemarco.Crawler.Person.Common;
internal interface IPersonCrawlerProvider
{
    string[] GetAvailableCrawlerNames();
    IPersonCrawler[] GetFilteredCrawlerInstances(string[] filter);
}

internal class PersonCrawlerProvider : IPersonCrawlerProvider
{
    public string[] GetAvailableCrawlerNames()
    {
        var result = GetOrderedCrawlerTypes()
            .Select(x => CrawlerInfo.FromCrawlerType(x).FriendlyName)
            .ToArray();
        return result;
    }

    public IPersonCrawler[] GetFilteredCrawlerInstances(string[] filter)
    {
        var types = GetOrderedCrawlerTypes();

        var result = types
            .Where(x =>
                filter.Length == 0 ||
                filter.Contains(CrawlerInfo.FromCrawlerType(x).FriendlyName))
            .Select(x => (IPersonCrawler)Activator.CreateInstance(x)!)
            .ToArray();
        return result;
    }

    internal static Type[] GetOrderedCrawlerTypes()
    {
        Type[] result =
        [
            .. typeof(IPersonCrawler).Assembly
                .GetTypes()
                .Where(x =>
                    x.IsAssignableTo(typeof(IPersonCrawler)) &&
                    x is { IsAbstract: false, IsClass: true} &&
                    x.GetCustomAttribute<CrawlerAttribute>() != null)
                .OrderBy(x => CrawlerInfo.FromCrawlerType(x).Priority)
        ];
        return result;
    }
}
