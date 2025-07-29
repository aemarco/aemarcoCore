namespace aemarco.Crawler.PersonTests._TestStuff;

internal abstract class PersonNameInfoCrawlerBase<T>
    where T : ISiteCrawler
{

    private readonly ISiteCrawler? _crawler;
    protected PersonNameInfoCrawlerBase()
    {
        var info = CrawlerInfo.FromCrawlerType(typeof(T));
        if (info.SkipTesting)
            return;

        _crawler = IocHelper.ResolveKeyed<ISiteCrawler>(typeof(T).Name);
    }

    [Test]
    public async Task GetPersonNameEntries_HasEntries()
    {
        if (_crawler is null)
        {
            TestHelper.NothingExpected(null);
            return;
        }

        var result = await _crawler.GetPersonNameEntries(CancellationToken.None);
        result.Length.Should().BeGreaterThan(0, "Expected at least 1 Entry");
        TestHelper.PrintPassed(result);
    }

}