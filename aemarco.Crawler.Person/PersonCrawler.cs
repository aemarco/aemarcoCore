namespace aemarco.Crawler.Person;

public class PersonCrawler
{

    private readonly List<string> _filterPersonSites = new();

    /// <summary>
    /// returns a list of crawler names, which are currently supported
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetAvailableCrawlers()
    {
        return GetAvailableCrawlerTypes().Select(type => type.ToCrawlerInfo().FriendlyName);
    }

    /// <summary>
    /// Not using means all sites will be crawled
    /// Using means only sites added will be crawled.
    /// Use GetAvailableCrawlers, to know which ones are supported
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
    public async Task<PersonInfo?> StartAsync(string nameToCrawl, CancellationToken cancellationToken = default)
    {
        //start all crawlers
        var tasks = new List<Task<PersonInfo>>();
        foreach (var type in GetAvailableCrawlerTypes())
        {
            //skip filtered
            if (_filterPersonSites.Count > 0 && !_filterPersonSites.Contains(type.ToCrawlerInfo().FriendlyName))
                continue;

            var crawler = (PersonCrawlerBase)Activator.CreateInstance(type, nameToCrawl)!;
            tasks.Add(Task.Run(() => crawler.GetPersonEntry(cancellationToken), cancellationToken));
        }

        //wait for being done
        var entries = new List<PersonInfo>();
        var errors = new List<Exception>();
        foreach (var task in tasks)
        {
            try
            {
                var personInfo = await task;
                entries.Add(personInfo);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }


        //merge entries together according to priority
        PersonInfo? result = null;
        foreach (var entry in entries
                     .OrderBy(x => x.PersonEntryPriority))
        {
            //first entry
            if (result == null)
            {
                result = entry;
                continue;
            }

            result.Merge(entry);
        }
        result?.Errors.AddRange(errors);
        return result;
    }

    internal static List<Type> GetAvailableCrawlerTypes()
    {
        var types = Assembly
            .GetAssembly(typeof(PersonCrawlerBase))!
            .GetTypes()
            .Where(x => x.IsAvailableCrawler())
            .OrderBy(x => x.ToCrawlerInfo().Priority)
            .ToList();
        return types;
    }
}