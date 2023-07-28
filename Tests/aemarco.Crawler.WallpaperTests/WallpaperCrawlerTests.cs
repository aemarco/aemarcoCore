﻿namespace aemarco.Crawler.WallpaperTests;

[SingleThreaded]
public class WallpaperCrawlerTests : TestBase
{

    [Explicit]
    [TestCaseSource(nameof(CrawlerCombinations))]
    public async Task Crawler_DoesWork_WithAuto(string site, string cat)
    {
        await WaitForSite(site);

        var crawler = GetAutoCrawler();
        crawler.AddSourceSiteFilter(site);
        crawler.AddCategoryFilter(cat);

        var result = await crawler.StartAsync();
        result.Should().NotBeNull();

        PrintJson(new { Albums = result.NewAlbums.Count });
        PrintJson(new { Entries = result.KnownEntries.Count });


        if (result.Warnings.FirstOrDefault() is { } warning)
            Assert.Warn(warning.ToString());
        else
            PrintJson(result);
    }




    [Test]
    public void GetAvailableSites_Delivers()
    {
        Sites.Count.Should().BeGreaterThan(0);
        PrintJson(Sites);
    }
    [Test]
    public void GetAvailableCategories_Delivers()
    {
        Cats.Count.Should().BeGreaterThan(0);
        PrintJson(Cats);
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
    public void HandleFilter_SiteFilter(string site)
    {
        //filter does filter
        var c = GetCrawler();
        c.AddSourceSiteFilter(site);
        c.HandleFilters();
        c._wallCrawlers.Select(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName).Should().BeEquivalentTo(site);

        OutputOffers(c);
    }
    [Test]
    public void HandleFilter_NoSiteFilter()
    {
        //no filter does not filter
        var all = GetCrawler();
        all.HandleFilters();
        all._wallCrawlers.Select(x => CrawlerInfo.FromCrawlerType(x.GetType()).FriendlyName).Should()
            .BeEquivalentTo(Sites);

        OutputOffers(all);
    }

    //cats
    private static List<string> Cats
    {
        get
        {
            var crawler = GetCrawler();
            var sc = crawler.GetAvailableCategories().ToList();
            return sc;
        }
    }
    [TestCaseSource(nameof(Cats))]
    public void HandleFilter_CatFilter(string cat)
    {
        //filter does filter
        var c = GetCrawler();
        c.AddCategoryFilter(cat);
        c.HandleFilters();

        c._wallCrawlers
            .All(x => x._crawlOffers is not null && x._crawlOffers
                .All(o => o.Category.Category == cat))
            .Should()
            .BeTrue();
        OutputOffers(c);
    }
    [Test]
    public void HandleFilter_NoCatFilter()
    {
        //no filter does not filter
        var all = GetCrawler();
        all.HandleFilters();

        all._wallCrawlers
            .All(x => x._crawlOffers is not null && x._crawlOffers.Any())
            .Should()
            .BeTrue();
        OutputOffers(all);
    }

    //combinations
    private static List<object> CrawlerCombinations
    {
        get
        {
            var result = new List<object>();
            foreach (var site in Sites)
            {
                var crawler = GetCrawler();
                crawler.AddSourceSiteFilter(site);
                crawler.HandleFilters();

                foreach (var cat in crawler.GetAvailableCategories())
                {
                    result.Add(new object[] { site, cat });
                }
            }
            return result;
        }
    }
    [TestCaseSource(nameof(CrawlerCombinations))]
    public async Task Crawler_DoesWork(string site, string cat)
    {
        await WaitForSite(site);

        var crawler = GetCrawler();
        crawler.AddSourceSiteFilter(site);
        crawler.AddCategoryFilter(cat);
        crawler.HandleFilters();

        var source = new CancellationTokenSource();
        var c = crawler._wallCrawlers.First();
        c.EntryFound += (_, _) => source.Cancel();
        try
        {
            await crawler.StartAsync(source.Token);
            Assert.Fail($"{site} - {cat} failed.");
        }
        catch (OperationCanceledException) { }


        if (c._result.Warnings.FirstOrDefault() is { } warning)
            Assert.Warn(warning.ToString());
        else if (c._result.NewEntries.FirstOrDefault() is { } wallEntry)
            PrintJson(wallEntry);
        else if (c._result.NewAlbums.FirstOrDefault() is { } albumEntry)
            PrintJson(albumEntry);
        else
            Assert.Fail($"{site} - {cat} found no entry.");
    }











    private static WallpaperCrawler GetCrawler()
    {
        var result = new WallpaperCrawler(1, 1);
        return result;
    }
    private static WallpaperCrawler GetAutoCrawler()
    {
        var result = new WallpaperCrawler();
        return result;
    }

    private readonly Random _random = new();
    private async Task WaitForSite(string site)
    {
        var min = 100;
        var max = 101;
        if (site is "Wallpaperscraft" or "Wallhaven")
        {
            min = 750;
            max = 2500;
        }
        await Task.Delay(_random.Next(min, max));
    }
    private static void OutputOffers(WallpaperCrawler crawler)
    {
        PrintJson(crawler._wallCrawlers
            .Where(x => x._crawlOffers is not null)
            .SelectMany(x => x._crawlOffers!)
            .Select(x => new
            {
                x.CategoryUri,
                x.SiteCategoryName,
                x.Category.Category
            }));
    }

}

public abstract class TestBase
{
    protected static void PrintJson(object? obj)
    {
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                obj,
                Formatting.Indented));
    }

}