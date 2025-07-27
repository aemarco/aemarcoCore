using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace aemarco.Crawler.Person;

public class PersonCrawler
{

    private readonly IPersonCrawlerProvider _personCrawlerProvider;
    private readonly ILogger<PersonCrawler> _logger;
    public PersonCrawler(ILogger<PersonCrawler> logger)
    {
        _personCrawlerProvider = new PersonCrawlerProvider();
        _logger = logger;
    }

    //without ioc
    public PersonCrawler()
    {
        _personCrawlerProvider = new PersonCrawlerProvider();
        _logger = NullLogger<PersonCrawler>.Instance;
    }

    //ctor for testing :(
    internal PersonCrawler(IPersonCrawlerProvider personCrawlerProvider)
    {
        _personCrawlerProvider = personCrawlerProvider;
        _logger = NullLogger<PersonCrawler>.Instance;
    }




    //filtering

    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    public IEnumerable<string> AvailableCrawlers => _personCrawlerProvider.GetAvailableCrawlerNames();

    private readonly List<string> _filterPersonSites = [];
    /// <summary>
    /// Not using means all sites will be crawled
    /// Using means only added sites will be crawled.
    /// </summary>
    /// <param name="crawler">crawler to include</param>
    public void AddPersonSiteFilter(string crawler)
    {
        if (!_filterPersonSites.Contains(crawler))
        {
            _filterPersonSites.Add(crawler);
        }
    }





    //crawling

    /// <summary>
    /// Do the names crawling :)
    /// </summary>
    /// <returns>list of PersonNameInfo</returns>
    public async Task<PersonNameInfo[]> CrawlPersonNames(CancellationToken cancellationToken = default)
    {
        var crawlers = _personCrawlerProvider.GetFilteredCrawlerInstances([.. _filterPersonSites]);

        //start all crawlers
        var tasks = crawlers
            .Select(x => (x.GetPersonNameEntries(cancellationToken), x))
            .ToArray();

        //wait for being done
        List<PersonNameInfo> list = [];
        foreach (var (task, cr) in tasks)
        {
            var crawlerInfo = cr.GetCrawlerInfo();
            try
            {
                var names = await task;
                list.AddRange(names);

                _logger.LogDebug("Crawler {crawlerInfo} found {count} names {@names}", crawlerInfo, names.Length, names);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Crawler {crawlerInfo} failed person names", crawlerInfo);
            }
        }

        //remove duplicates and order
        PersonNameInfo[] result = [
                .. list
                    .Distinct()
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
            ];
        _logger.LogInformation("CrawlPersonNames found {count} names {@names}", result.Length, result);
        return result;
    }

    /// <summary>
    /// Do the details crawling :)
    /// </summary>
    /// <returns>composed PersonInfo</returns>
    public async Task<PersonInfo> CrawlPerson(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        firstName = firstName.TitleCase();
        lastName = lastName.TitleCase();
        var result = new PersonInfo
        {
            FirstName = firstName,
            LastName = lastName
        };


        var crawlers = _personCrawlerProvider.GetFilteredCrawlerInstances([.. _filterPersonSites]);

        //start all crawlers
        var tasks = crawlers
            .Select(x => (x.GetPersonEntry(firstName, lastName, cancellationToken), x))
            .ToArray();

        //wait for being done
        List<PersonInfo> entries = [];
        foreach (var (task, cr) in tasks)
        {
            var crawlerInfo = cr.GetCrawlerInfo();
            try
            {
                var personInfo = await task;
                entries.Add(personInfo);

                _logger.LogDebug("Crawler {crawlerInfo} found info {@personInfo}", crawlerInfo, personInfo);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex);
                _logger.LogError(ex, "Crawler {crawlerInfo} failed person details", crawlerInfo);
            }
        }

        result.Merge(entries);
        _logger.LogInformation("CrawlPerson found info {@personInfo}", result);

        return result;
    }

}