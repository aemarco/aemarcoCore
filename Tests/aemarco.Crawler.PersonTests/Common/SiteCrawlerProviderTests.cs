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

        result.Should().Equal(expected);

        TestHelper.PrintPassed(result);
    }

    [Test]
    public void GetFilteredCrawlerInstances_ShouldFilter()
    {
        var sut = IocHelper.Resolve<ISiteCrawlerProvider>();

        var result = sut.GetFilteredCrawlerInstances(["Nudevista"]);

        result.Should()
                .HaveCount(1)
            .And.OnlyContain(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName == "Nudevista");

        TestHelper.PrintPassed(result.Select(x => CrawlerInfo.FromCrawlerType(x.GetType())));
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

        result.Should()
                .HaveCount(expectedCount)
            .And.OnlyHaveUniqueItems();


        TestHelper.PrintPassed(result.Select(x => CrawlerInfo.FromCrawlerType(x.GetType())));
    }

}

