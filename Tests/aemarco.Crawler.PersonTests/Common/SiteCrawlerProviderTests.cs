namespace aemarco.Crawler.PersonTests.Common;

public class SiteCrawlerProviderTests
{

    [Test]
    public void GetAvailableCrawlerNames_DeliversNames()
    {
        var sut = IocHelper.Resolve<ISiteCrawlerProvider>();
        var expected = typeof(ISiteCrawler).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<CrawlerAttribute>() != null)
            .Select(CrawlerInfo.FromCrawlerType)
            .OrderBy(x => x.Priority)
            .Select(x => x.FriendlyName)
            .ToArray();

        var result = sut.GetAvailableCrawlerNames();

        result.ShouldBe(expected);

        result.Dump();
    }

    [Test]
    public void GetFilteredCrawlerInstances_ShouldFilter()
    {
        var sut = IocHelper.Resolve<ISiteCrawlerProvider>();

        var result = sut.GetFilteredCrawlerInstances(["Nudevista"]);

        result.ShouldHaveSingleItem();
        result.ShouldAllBe(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName == "Nudevista");

        result.Select(x => CrawlerInfo.FromCrawlerType(x.GetType())).Dump();
    }

    [Test]
    public void GetFilteredCrawlerInstances_DeliversAllWithoutFilter()
    {
        var sut = IocHelper.Resolve<ISiteCrawlerProvider>();
        var expectedCount = typeof(ISiteCrawler).Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttribute<CrawlerAttribute>() != null)
            .Select(CrawlerInfo.FromCrawlerType)
            .OrderBy(x => x.Priority)
            .Select(x => x.FriendlyName)
            .Count();

        var result = sut.GetFilteredCrawlerInstances([]);

        result.Length.ShouldBe(expectedCount);
        result.ShouldBeUnique();


        result.Select(x => CrawlerInfo.FromCrawlerType(x.GetType())).Dump();
    }

}

