using aemarco.Crawler.Common;

namespace aemarco.Crawler.PersonTests.Common;
public class PersonCrawlerProviderTests
{

    [Test]
    public void GetAvailableCrawlerNames()
    {
        var sut = new PersonCrawlerProvider();

        var expected = PersonCrawlerProvider.GetOrderedCrawlerTypes()
            .Select(CrawlerInfo.FromCrawlerType)
            .Select(x => x.FriendlyName)
            .ToArray();

        var result = sut.GetAvailableCrawlerNames();
        result.Should().Equal(expected);
    }

    [Test]
    public void GetFilteredCrawlerInstances_ShouldDeliverOnlyCrawlerTypes()
    {
        var sut = new PersonCrawlerProvider();

        var result = sut.GetFilteredCrawlerInstances(["Nudevista"]);

        result.Should().HaveCount(1);
        result.Should().OnlyContain(x => x is Nudevista);
    }

    [Test]
    public void GetOrderedCrawlerTypes_ShouldDeliverOnlyCrawlerTypes()
    {
        var result = PersonCrawlerProvider.GetOrderedCrawlerTypes();

        result.Should().NotBeEmpty()
            .And.OnlyHaveUniqueItems()
            .And.OnlyContain(x => x.IsAssignableTo(typeof(IPersonCrawler)))
            .And.OnlyContain(x => !x.IsAbstract)
            .And.OnlyContain(x => x.IsClass)
            .And.OnlyContain(x => x.GetCustomAttribute<CrawlerAttribute>() != null);
    }

    [Test]
    public void GetOrderedCrawlerTypes_ShouldBeOrdered()
    {
        var result = PersonCrawlerProvider.GetOrderedCrawlerTypes();
        result
            .Select(CrawlerInfo.FromCrawlerType)
            .Select(x => x.Priority)
            .Should()
            .BeInAscendingOrder();
    }
}

