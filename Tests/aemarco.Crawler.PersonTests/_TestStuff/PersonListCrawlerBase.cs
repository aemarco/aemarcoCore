using aemarco.TestBasics;
using System.Threading;

namespace aemarco.Crawler.PersonTests._TestStuff;

internal abstract class PersonListCrawlerBase<T>
{

    private readonly CrawlerInfo _crawlerInfo = CrawlerInfo.FromCrawlerType(typeof(T));


    [Test]
    public async Task HandleGirlList_HasEntries()
    {
        if (_crawlerInfo.SkipTesting)
        {
            TestHelper.NothingExpected(null);
            return;
        }


        var crawler = new PersonCrawler();
        crawler.AddPersonSiteFilter(_crawlerInfo.FriendlyName);
        var result = await crawler.CrawlPersonList(CancellationToken.None);

        result.Length.Should().BeGreaterThan(0, "Expected at least 1 Entry");
    }

}