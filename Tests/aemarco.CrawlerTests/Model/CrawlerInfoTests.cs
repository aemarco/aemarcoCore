using aemarco.Crawler.Common;
using aemarco.Crawler.Model;

namespace aemarco.CrawlerTests.Model;
public class CrawlerInfoTests
{

    [Test]
    public void ToString_Works()
    {
        var candidate = new CrawlerInfo("Test", 0);

        var result = candidate.ToString();

        result.Should().Be("Test");
    }

    [Test]
    public void FromCrawlerType_Delivers()
    {
        var result = CrawlerInfo.FromCrawlerType(typeof(TestClass));
        result.Should().Be(new CrawlerInfo("Test", 0));
    }


}


[Crawler("Test", 0)]
class TestClass
{

}
