namespace aemarco.Crawler.Person;

public interface IPersonCrawler
{

    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    IEnumerable<string> AvailableCrawlers { get; }

    /// <summary>
    /// Not using means all sites will be crawled
    /// Using means only added sites will be crawled.
    /// </summary>
    /// <param name="crawler">crawler to include</param>
    void AddPersonSiteFilter(string crawler);

    /// <summary>
    /// Do the names crawling :)
    /// </summary>
    /// <returns>list of PersonNameInfo</returns>
    Task<PersonName[]> CrawlPersonNames(CancellationToken cancellationToken = default);

    /// <summary>
    /// Do the details crawling :)
    /// </summary>
    /// <returns>composed PersonInfo</returns>
    Task<PersonInfo> CrawlPerson(string firstName, string lastName, CancellationToken cancellationToken = default);

}