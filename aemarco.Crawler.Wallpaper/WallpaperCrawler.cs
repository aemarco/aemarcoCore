// ReSharper disable UnusedMethodReturnValue.Global

namespace aemarco.Crawler.Wallpaper;

public class WallpaperCrawler
{

    // ReSharper disable once InconsistentNaming
    internal readonly List<WallpaperCrawlerBasis> _wallCrawlers;
    public WallpaperCrawler(int? startPage = null, int? lastPage = null)
    {
        if (startPage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startPage), startPage, "Can´t be lower than 1");
        }
        var start = startPage ?? 1;
        if (lastPage < start)
        {
            throw new ArgumentOutOfRangeException(nameof(lastPage), lastPage, "Can´t be smaller than startPage");
        }
        var end = lastPage ?? 10;
        var news = !startPage.HasValue && !lastPage.HasValue;



        _wallCrawlers = new List<WallpaperCrawlerBasis>();
        foreach (var type in Assembly
                     .GetAssembly(typeof(WallpaperCrawlerBasis))!
                     .GetTypes()
                     .Where(x => x.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
                     .Where(x => CrawlerInfo.FromCrawlerType(x).IsAvailable))
        {
            var crawler = (WallpaperCrawlerBasis)(Activator.CreateInstance(type, start, end, news)
                          ?? throw new Exception($"Could not activate {type.FullName}"));
            _wallCrawlers.Add(crawler);
        }
    }


    //configure
    private Func<List<string>>? _knownUrlsFunc;
    public void Configure(Func<List<string>>? knownUrlsFunc = null)
    {
        _knownUrlsFunc = knownUrlsFunc;
    }




    //filter sites
    private readonly List<string> _filterSourceSites = new();
    public IEnumerable<string> GetAvailableSourceSites()
    {
        foreach (var info in _wallCrawlers
                     .Select(x => CrawlerInfo.FromCrawlerType(x.GetType())))
        {
            yield return info.FriendlyName;
        }
    }
    /// <summary>
    /// Used to filter which sites to limit to
    /// Not using means all sites will be crawled
    /// Using means only sites added will be crawled
    /// use GetAvailableSourceSites() to know, which are available
    /// </summary>
    /// <param name="sourceSite"></param>
    public void AddSourceSiteFilter(string sourceSite)
    {
        if (!_filterSourceSites.Contains(sourceSite))
        {
            _filterSourceSites.Add(sourceSite);
        }
    }



    //filter categories
    private readonly List<string> _filterCategories = new();
    public IEnumerable<string> GetAvailableCategories()
    {
        var categories = new List<string>();
        foreach (var crawler in _wallCrawlers
                     .Where(x => _filterSourceSites.Count == 0 || _filterSourceSites.Contains(CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName)))
        {
            foreach (var offer in crawler.GetOffers())
            {
                //skip those already given
                if (categories.Contains(offer.Category.Category))
                    continue;
                categories.Add(offer.Category.Category);


                yield return offer.Category.Category;
            }
        }
    }
    /// <summary>
    /// Used to filter which results being of interest to crawl
    /// Not using this means everything will be crawled
    /// Using this means only added categories will be crawled
    /// </summary>
    public void AddCategoryFilter(string category)
    {
        if (!_filterCategories.Contains(category))
        {
            _filterCategories.Add(category);
        }
    }








    /// <summary>
    /// Do the crawling :)
    /// events may be used for data and completion
    /// </summary>
    public async Task<WallCrawlerResult> StartAsync(CancellationToken cancellationToken = default)
    {
        HandleFilters();

        //start all crawlers
        var known = _knownUrlsFunc?.Invoke() ?? new List<string>();
        var tasks = new List<Task<WallCrawlerResult>>();
        //creates all available crawlers and adds them if applicable
        foreach (var crawler in _wallCrawlers)
        {
            tasks.Add(Task.Run(() => crawler.Start(
                    known,
                    cancellationToken),
                cancellationToken));
        }


        //wait for being done
        await Task.WhenAll(tasks);
        var entries = new List<WallCrawlerResult>();
        foreach (var task in tasks)
        {
            var crawlResult = await task;
            entries.Add(crawlResult);
        }

        //merge entries together
        var result = new WallCrawlerResult
        {
            NumberOfCrawlersInvolved = entries.Sum(x => x.NumberOfCrawlersInvolved),
            NewEntries = entries.SelectMany(x => x.NewEntries).ToList(),
            KnownEntries = entries.SelectMany(x => x.KnownEntries).ToList(),
            NewAlbums = entries.SelectMany(x => x.NewAlbums).ToList(),
            KnownAlbums = entries.SelectMany(x => x.KnownAlbums).ToList(),
            Warnings = entries.SelectMany(x => x.Warnings).ToList()
        };
        return result;
    }


    internal void HandleFilters()
    {
        //handle wallpaper site filter
        _wallCrawlers.RemoveAll(c =>
        {
            //we don´t filter on sites, so all stay
            if (_filterSourceSites.Count == 0)
                return false;

            //we filter on sites, so only those stay, which are contained in the filter
            var info = CrawlerInfo.FromCrawlerType(c.GetType());
            return !_filterSourceSites.Contains(info.FriendlyName);

        });

        //handle wallpaper category filter
        _wallCrawlers.RemoveAll(c =>
        {
            //we don´t filter on categories, so all stay
            if (_filterCategories.Count == 0)
                return false;

            //we filter on categories, so only those stay, which have a remaining offer
            c.LimitAsPerFilterList(_filterCategories);
            return !c.GetOffers().Any();
        });

        //even if no filter, we remove all which offer nothing
        _wallCrawlers.RemoveAll(c => !c.GetOffers().Any());
    }

}