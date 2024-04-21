namespace aemarco.Crawler.Person;

public class PersonCrawler
{

    private readonly Type[] _crawlerTypes;
    public PersonCrawler()
    {
        _crawlerTypes = [.. typeof(IPersonCrawler).Assembly
            .GetTypes()
            .Where(x =>
                x.IsAssignableTo(typeof(IPersonCrawler)) &&
                x is { IsAbstract: false, IsClass: true} &&
                x.GetCustomAttribute<CrawlerAttribute>() != null)
            .OrderBy(x => CrawlerInfo.FromCrawlerType(x).Priority)];
    }

    /// <summary>
    /// list of crawler names, which are currently supported
    /// </summary>
    public IEnumerable<string> AvailableCrawlers => _crawlerTypes
        .Select(x => CrawlerInfo.FromCrawlerType(x).FriendlyName);

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


        //start all crawlers
        var tasks = _crawlerTypes
            .Where(x =>
                _filterPersonSites.Count == 0 ||
                _filterPersonSites.Contains(CrawlerInfo.FromCrawlerType(x).FriendlyName))
            .Select(x => (IPersonCrawler)Activator.CreateInstance(x)!)
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
                var personInfo = new PersonInfo();
                personInfo.Errors.Add(ex);
                entries.Add(personInfo);
            }
        }

        var result = new PersonInfo
        {
            FirstName = firstName,
            LastName = lastName
        };
        foreach (var entry in entries
                     .OrderBy(x => x.CrawlerInfos.FirstOrDefault()?.Priority ?? int.MaxValue))
        {
            result.Merge(entry);
        }
        return result;
    }

}