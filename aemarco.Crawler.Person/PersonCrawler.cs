namespace aemarco.Crawler.Person;

public class PersonCrawler
{

    private readonly IPersonCrawlerProvider _personCrawlerProvider;
    internal PersonCrawler(IPersonCrawlerProvider personCrawlerProvider)
    {
        _personCrawlerProvider = personCrawlerProvider;
    }
    public PersonCrawler() : this(
            new PersonCrawlerProvider())
    { }

    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    public IEnumerable<string> AvailableCrawlers => _personCrawlerProvider.GetAvailableCrawlerNames();

    private readonly List<string> _filterPersonSites = [];
    /// <summary>
    /// Not using means all sites will be crawled
    /// Using means only sites added will be crawled.
    /// </summary>
    /// <param name="crawler">crawler to include</param>
    public void AddPersonSiteFilter(string crawler)
    {
        if (!_filterPersonSites.Contains(crawler))
        {
            _filterPersonSites.Add(crawler);
        }
    }

    /// <summary>
    /// Do the crawling :)
    /// </summary>
    /// <returns>composed PersonEntry</returns>
    public async Task<PersonInfo> StartAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        firstName = firstName.TitleCase();
        lastName = lastName.TitleCase();
        var result = new PersonInfo
        {
            FirstName = firstName,
            LastName = lastName
        };


        var crawlers = _personCrawlerProvider
            .GetFilteredCrawlerInstances([.. _filterPersonSites]);

        //start all crawlers
        var tasks = crawlers
            .Select(x => x.GetPersonEntry(firstName, lastName, cancellationToken))
            .ToArray();

        //wait for being done
        List<PersonInfo> entries = [];
        foreach (var task in tasks)
        {
            try
            {
                var personInfo = await task;
                entries.Add(personInfo);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex);
            }
        }
        result.Merge(entries);
        return result;
    }

}