namespace aemarco.Crawler.Person.Crawlers.Obsolete;

[PersonCrawler("DummyCrawler", int.MaxValue)]
internal class DummyCrawler : PersonCrawlerBase
{
    public DummyCrawler(string nameToCrawl)
        : base(nameToCrawl)
    { }

    internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
    {
        throw new Exception("Disabled DummyCrawler was started");
    }
}