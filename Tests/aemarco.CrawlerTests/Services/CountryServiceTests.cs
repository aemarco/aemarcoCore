using aemarco.Crawler.Services;

namespace aemarco.CrawlerTests.Services;
internal class CountryServiceTests : TestBase
{

    [TestCase(null, null)]
    [TestCase("Canada", "Canada")]
    [TestCase("Hungary", "Hungary")]
    [TestCase("United States", "United States")]
    [TestCase("American", "United States")]
    [TestCase("USA", "United States")]
    [TestCase("US", "United States")]
    [TestCase("Russian Federation", "Russia")]
    [TestCase("Russia", "Russia")]
    [TestCase("Russian", "Russia")]
    public void FindCountry(string? text, string? expected)
    {
        var result = new CountryService().FindCountry(text);
        result.Should().Be(expected);
        PrintJson(result);
    }


    [Test]
    public void GetData_DeliversAtLeastOne()
    {
        Country[] result = [.. new CountryService().GetData()];
        result.Length.Should().BeGreaterThan(0);
        PrintJson(result);
    }

}
