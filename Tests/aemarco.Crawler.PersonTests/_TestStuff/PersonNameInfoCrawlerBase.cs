namespace aemarco.Crawler.PersonTests._TestStuff;

[Category("LiveSite")] //CI excludes it, see build/Build.cs Tests
internal abstract class PersonNameInfoCrawlerBase<T>
    where T : ISiteCrawler
{

    private readonly ISiteCrawler? _crawler;
    protected PersonNameInfoCrawlerBase()
    {
        if (typeof(T).GetCustomAttribute<SkipTestingAttribute>() is not null)
            return;

        _crawler = IocHelper.ResolveKeyed<ISiteCrawler>(typeof(T).Name);
    }

    [Test]
    public async Task GetPersonNameEntries_HasEntries()
    {
        if (_crawler is null)
        {
            TestOutput.NothingExpected(null);
            return;
        }

        var result = await _crawler.GetPersonNameEntries(CancellationToken.None);
        result.Length.Should().BeGreaterThan(0, "Expected at least 1 Entry");
        result.Dump();
    }

}