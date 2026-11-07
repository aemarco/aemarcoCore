using aemarco.Crawler.Wallpaper.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace aemarco.Crawler.WallpaperTests;

[Category("LiveSite")] //CI excludes it, see build/Build.cs Tests
public class WallpaperCrawlerTests
{

    [Test]
    public void GetAvailableSites_Delivers()
    {
        Sites.Count.ShouldBeGreaterThan(0);
        Sites.Dump();
    }
    [Test]
    public async Task GetAvailableCategories_Delivers()
    {
        var crawler = GetCrawler();

        var cats = await crawler.GetAvailableCategories();

        cats.Count.ShouldBeGreaterThan(0);
        cats.Dump();
    }

    //sites
    private static List<string> Sites
    {
        get
        {
            var crawler = GetCrawler();
            var ss = crawler.GetAvailableSourceSites().ToList();
            return ss;
        }
    }
    [TestCaseSource(nameof(Sites))]
    public async Task HandleFilter_SiteFilter(string site)
    {
        //filter does filter
        var c = GetCrawler();
        c.AddSourceSiteFilter(site);
        await c.HandleFilters();
        c._wallCrawlers.Select(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName).ShouldBe([site], true);

        OutputOffers(c);
    }
    [Test]
    public async Task HandleFilter_NoSiteFilter()
    {
        //no filter does not filter
        var all = GetCrawler();
        await all.HandleFilters();
        all._wallCrawlers.Select(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName).ShouldBe(Sites, true);

        OutputOffers(all);
    }

    //cats
    private static async IAsyncEnumerable<string> Cats()
    {
        var crawler = GetCrawler();
        foreach (var cat in await crawler.GetAvailableCategories())
            yield return cat;
    }
    //NUnit 4 takes async enumerables as sources, the ReSharper inspection does not know that yet
    // ReSharper disable once NUnit.TestCaseSourceShouldImplementIEnumerable
    [TestCaseSource(nameof(Cats))]
    public async Task HandleFilter_CatFilter(string cat)
    {
        //filter does filter
        var c = GetCrawler();
        c.AddCategoryFilter(cat);
        await c.HandleFilters();

        c._wallCrawlers
            .All(x => x._crawlOffers is not null && x._crawlOffers
                .All(o => o.Category.Category == cat))
            .ShouldBeTrue();
        OutputOffers(c);
    }
    [Test]
    public async Task HandleFilter_NoCatFilter()
    {
        //no filter does not filter
        var all = GetCrawler();
        await all.HandleFilters();

        all._wallCrawlers
            .All(x => x._crawlOffers is not null && x._crawlOffers.Count > 0)
            .ShouldBeTrue();
        OutputOffers(all);
    }

    //combinations
    private static async IAsyncEnumerable<object[]> CrawlerCombinations()
    {
        foreach (var site in Sites)
        {
            var crawler = GetCrawler();
            crawler.AddSourceSiteFilter(site);
            await crawler.HandleFilters();

            foreach (var cat in await crawler.GetAvailableCategories())
                yield return [site, cat];
        }
    }
    //NUnit 4 takes async enumerables as sources, the ReSharper inspection does not know that yet
    // ReSharper disable once NUnit.TestCaseSourceShouldImplementIEnumerable
    [TestCaseSource(nameof(CrawlerCombinations))]
    public async Task Crawler_DoesWork(string site, string cat)
    {
        var crawler = GetCrawler();
        crawler.AddSourceSiteFilter(site);
        crawler.AddCategoryFilter(cat);
        await crawler.HandleFilters();

        var source = new CancellationTokenSource();

        crawler.EntryFound += (_, _) => source.Cancel();
        try
        {
            await crawler.CrawlWallpapers(1, 1, source.Token);
            Assert.Fail($"{site} - {cat} failed.");
        }
        catch (OperationCanceledException) { }

        //cancelled while the offer was running, so what it found is still with the offer
        var c = crawler._wallCrawlers.First();
        var offers = c._crawlOffers!;
        if (c._result.Warnings.FirstOrDefault() is { } warning)
            Assert.Warn(warning.ToString());
        else if (offers.SelectMany(x => x.NewEntries).FirstOrDefault() is { } wallEntry)
            wallEntry.Dump();
        else if (offers.SelectMany(x => x.NewAlbums).FirstOrDefault() is { } albumEntry)
            albumEntry.Dump();
        else
            Assert.Fail($"{site} - {cat} found no entry.");
    }











    //one provider for all tests, so tests running in parallel share one rate limiting (one pace per domain);
    //Wallhaven takes part only with a key, set it locally as environment variable (never in the repo)
    private static readonly ServiceProvider Services = new ServiceCollection()
        .AddWallCrawler(x =>
            x.WallhavenApiKey = Environment.GetEnvironmentVariable("WALLHAVEN_APIKEY"))
        //tests always use the NullLogger
        .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
        .BuildServiceProvider();
    private static WallpaperCrawler GetCrawler() =>
        (WallpaperCrawler)Services.GetRequiredService<IWallpaperCrawler>();

    [OneTimeTearDown]
    public async Task DisposeServices() => await Services.DisposeAsync();

    private static void OutputOffers(WallpaperCrawler crawler)
    {
        crawler._wallCrawlers
            .Where(x => x._crawlOffers is not null)
            .SelectMany(x => x._crawlOffers!)
            .Select(x => new
            {
                x.CategoryUri,
                x.SiteCategoryName,
                x.Category.Category
            })
            .Dump();
    }

}
