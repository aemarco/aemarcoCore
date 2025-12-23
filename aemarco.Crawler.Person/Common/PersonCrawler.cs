namespace aemarco.Crawler.Person.Common;

internal class PersonCrawler : IPersonCrawler
{

    private readonly ISiteCrawlerProvider _siteCrawlerProvider;
    private readonly ILogger<PersonCrawler> _logger;
    public PersonCrawler(
        ISiteCrawlerProvider siteCrawlerProvider,
        ILogger<PersonCrawler> logger)
    {
        _siteCrawlerProvider = siteCrawlerProvider;
        _logger = logger;
    }


    //filtering

    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    public IEnumerable<string> AvailableCrawlers => _siteCrawlerProvider.GetAvailableCrawlerNames();

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
    public async Task<PersonName[]> CrawlPersonNames(CancellationToken cancellationToken = default)
    {
        var crawlers = _siteCrawlerProvider.GetFilteredCrawlerInstances([.. _filterPersonSites]);

        //start all crawlers
        var tasks = crawlers
            .Select(x => (x.GetPersonNameEntries(cancellationToken), x))
            .ToArray();

        //wait for being done
        List<PersonName> list = [];
        foreach (var (task, cr) in tasks)
        {
            try
            {
                var names = await task;
                list.AddRange(names);

                _logger.LogDebug("Crawler {crawlerType} found {count} names {@names}", cr.GetType().Name, names.Length, names);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Crawler {crawlerType} failed person names", cr.GetType().Name);
            }
        }

        //remove duplicates and order
        PersonName[] result = [
                .. list
                    .Distinct()
                    .OrderBy(x => x.Name)
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


        var crawlers = _siteCrawlerProvider.GetFilteredCrawlerInstances([.. _filterPersonSites]);

        //start all crawlers
        var tasks = crawlers
            .Select(x => (x.GetPersonEntry(firstName, lastName, cancellationToken), x))
            .ToArray();

        //wait for being done
        List<PersonInfo> entries = [];
        foreach (var (task, cr) in tasks)
        {
            try
            {
                var personInfo = await task;
                entries.Add(personInfo);

                _logger.LogDebug("Crawler {crawlerType} found info {@personInfo}", cr.GetType().Name, personInfo);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex);
                _logger.LogError(ex, "Crawler {crawlerType} failed person details", cr.GetType().Name);
            }
        }

        result.Merge(entries);
        _logger.LogInformation("CrawlPerson found info {@personInfo}", result);

        return result;
    }

}