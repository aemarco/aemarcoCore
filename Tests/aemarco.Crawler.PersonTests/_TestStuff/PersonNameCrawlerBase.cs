using aemarco.TestBasics;
using System.Threading;

namespace aemarco.Crawler.PersonTests._TestStuff;

internal abstract class PersonNameCrawlerBase<T>
{
    private readonly PersonCrawlerProvider _provider = new();
    private readonly CrawlerInfo _crawlerInfo = CrawlerInfo.FromCrawlerType(typeof(T));




    [Test]
    public async Task GetPersonNameEntries_HasEntries()
    {
        if (_crawlerInfo.SkipTesting)
        {
            TestHelper.NothingExpected(null);
            return;
        }

        if (_provider.GetFilteredCrawlerInstances([_crawlerInfo.FriendlyName]).SingleOrDefault()
                is not { } cr)
        {
            Assert.Warn("Crawler provider does not deliver 1 crawler");
            return;
        }


        var result = await cr.GetPersonNameEntries(CancellationToken.None);
        result.Length.Should().BeGreaterThan(0, "Expected at least 1 Entry");
        TestHelper.PrintPassed(result);
    }





}