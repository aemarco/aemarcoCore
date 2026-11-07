using aemarco.Crawler.Configuration;
using aemarco.Crawler.Wallpaper.Clients;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Contracts.Clients;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace aemarco.Crawler.WallpaperTests;

//offline: the api is served from saved real responses (Resources/TestData/Wallhaven)
public class WallhavenTests
{

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("some-key", true)]
    public void AvailableSourceSites_ApiKey_WallhavenOnlyWithKey(string? apiKey, bool expected)
    {
        var sut = GetCrawler(apiKey, null);

        var result = sut.AvailableSourceSites.Contains("Wallhaven");

        result.ShouldBe(expected);
    }

    [Test]
    public async Task AvailableCategories_Wallhaven_GeneralGoesToNone()
    {
        var sut = GetCrawler("some-key", null);
        sut.AddSourceSiteFilter("Wallhaven");

        var result = await sut.GetAvailableCategories();

        //each internal category is one crawl row at the consumer
        result.ShouldBe(["None", "Girls_Fantasy", "Girls"], true);
    }

    [Test]
    public async Task CrawlWallpapers_ApiResponses_NewWallsWithTags()
    {
        var handler = new FixtureHandler();
        var sut = GetCrawler("some-key", handler);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 1);

        //all three people offers (SFW, Sketchy, NSFW) see the same page, the later ones find them known
        result.Warnings.ShouldBeEmpty();
        result.NewEntries.Select(x => x.Url).ShouldBe(
        [
            "https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg",
            "https://w.wallhaven.cc/full/ly/wallhaven-lyryk2.jpg",
            "https://w.wallhaven.cc/full/gw/wallhaven-gwvw9d.jpg"
        ]);
        result.KnownEntries.Count.ShouldBe(6);
        result.NewEntries[0].FileName.ShouldBe("wallhaven-vp3pd5");
        result.NewEntries[0].ThumbnailUrl.ShouldBe("https://th.wallhaven.cc/small/vp/vp3pd5.jpg");
        result.NewEntries[0].Tags.ShouldContain("redhead");
        result.NewEntries[1].Tags.ShouldContain("science fiction");
        handler.Requests.ShouldAllBe(x => x.Headers.GetValues("X-API-Key").Single() == "some-key");
        handler.Requests
            .Where(x => x.RequestUri!.AbsolutePath.EndsWith("/search"))
            .Select(x => HttpUtility.ParseQueryString(x.RequestUri!.Query)["purity"])
            .ShouldBe(["100", "010", "001"]);
        result.Dump();
    }

    [Test]
    public async Task CrawlWallpapers_KnownWall_NoDetailCall()
    {
        var handler = new FixtureHandler();
        var sut = GetCrawler("some-key", handler);
        sut.Configure(() => ["https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg"]);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 1);

        result.NewEntries.Select(x => x.Url).ShouldNotContain("https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg");
        handler.Requests.Select(x => x.RequestUri!.AbsolutePath).ShouldNotContain("/api/v1/w/vp3pd5");
        handler.Requests.Select(x => x.RequestUri!.AbsolutePath).ShouldContain("/api/v1/w/lyryk2");
    }


    private static WallpaperCrawler GetCrawler(string? apiKey, HttpMessageHandler? handler)
    {
        //the stub needs no pacing, rate limiting is tested with the core pipeline
        var services = new ServiceCollection()
            .AddCoreCrawler(x => x.Domains["wallhaven.cc"] = new RateLimit(1000, TimeSpan.FromSeconds(1)))
            .AddWallCrawler(x => x.WallhavenApiKey = apiKey);
        if (handler is not null)
            services
                .AddHttpClient<IWallhavenClient, WallhavenClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler);

        var result = (WallpaperCrawler)services
            .BuildServiceProvider()
            .GetRequiredService<IWallpaperCrawler>();
        return result;
    }

    private static FileInfo GetFixture(string name) =>
        TestDataFile.GetFixture("Resources", "TestData", "Wallhaven", name);

    //answers search with search.json and w/{id} with w_{id}.json
    private class FixtureHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);

            var path = request.RequestUri!.AbsolutePath;
            var fixture = path.EndsWith("/search")
                ? GetFixture("search.json")
                : GetFixture($"w_{path[(path.LastIndexOf('/') + 1)..]}.json");
            if (!fixture.Exists)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var json = await File.ReadAllTextAsync(fixture.FullName, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }

}
