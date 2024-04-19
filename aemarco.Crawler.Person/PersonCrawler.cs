namespace aemarco.Crawler.Person;

public class PersonCrawler
{

    private readonly List<string> _filterPersonSites = new();
    /// <summary>
    /// returns a list of crawler names, which are currently supported
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetAvailableCrawlers()
    {
        return GetAvailableCrawlerTypes()
            .Select(t => CrawlerInfo.FromCrawlerType(t).FriendlyName);
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
    /// 
    /// </summary>
    /// <returns>composed PersonEntry</returns>
    public async Task<PersonInfo> StartAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        firstName = firstName.TitleCase();
        lastName = lastName.TitleCase();


        //start all crawlers
        var tasks = new List<Task<PersonInfo>>();
        foreach (var type in GetAvailableCrawlerTypes())
        {
            //skip filtered
            if (_filterPersonSites.Count > 0 &&
                !_filterPersonSites.Contains(CrawlerInfo.FromCrawlerType(type).FriendlyName))
                continue;


            var crawler = (PersonCrawlerBase)Activator.CreateInstance(type)!;
            tasks.Add(Task.Run(() =>
                crawler.GetPersonEntry(firstName, lastName, cancellationToken), cancellationToken));
        }
        //wait for being done
        var entries = new List<PersonInfo>();
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












    private static List<Type> GetAvailableCrawlerTypes()
    {
        var types = Assembly
            .GetAssembly(typeof(PersonCrawlerBase))!
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(PersonCrawlerBase)))
            .OrderBy(x => CrawlerInfo.FromCrawlerType(x).Priority)
            .ToList();
        return types;
    }

}