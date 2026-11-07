using aemarco.Crawler.Configuration;
using aemarco.Crawler.Wallpaper.Clients;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Contracts.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        result.NewEntries[0].ThumbnailUrl.ShouldBe("https://th.wallhaven.cc/orig/vp/vp3pd5.jpg");
        result.NewEntries[0].Tags.Select(x => x.Text).ShouldContain("redhead");
        result.NewEntries[1].Tags.Select(x => x.Text).ShouldContain("science fiction");
        result.NewEntries[0].Tags.ShouldContain(x => x.Text == "Tsuki (Billlie)"
            && x.Category == "Musicians"
            && x.Aliases.SequenceEqual(new[] { "Fukutomi Tsuki", "TARU", "福富 つき" }));
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
        sut.Configure((_) => Task.FromResult<IEnumerable<string>>(["https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg"]));
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 1);

        result.NewEntries.Select(x => x.Url).ShouldNotContain("https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg");
        handler.Requests.Select(x => x.RequestUri!.AbsolutePath).ShouldNotContain("/api/v1/w/vp3pd5");
        handler.Requests.Select(x => x.RequestUri!.AbsolutePath).ShouldContain("/api/v1/w/lyryk2");
    }

    [Test]
    public async Task CrawlWallpapers_KnownUrlsFunc_AskedOncePerCrawlWithTokenAndListUntouched()
    {
        var sut = GetCrawler("some-key", new FixtureHandler());
        List<string> known = ["https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg"];
        var calls = 0;
        CancellationToken? received = null;
        sut.Configure((token) =>
        {
            calls++;
            received = token;
            return Task.FromResult<IEnumerable<string>>(known);
        });
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");
        using var cts = new CancellationTokenSource();

        await sut.CrawlWallpapers(1, 1, cts.Token);

        //one call for all offers, and what the crawlers found does not end up in the consumer's list
        calls.ShouldBe(1);
        received.ShouldBe(cts.Token);
        known.ShouldBe(["https://w.wallhaven.cc/full/vp/wallhaven-vp3pd5.jpg"]);
    }


    //the offer is the unit: entries of pages before a failure must not be delivered, the next crawl
    //would find them known, and its streak of known entries would end before the page which was missed
    [Test]
    public async Task CrawlWallpapers_SearchFailsOnLaterPage_OfferDeliversNothing()
    {
        var handler = new FixtureHandler
        {
            LastPage = 2,
            Fail = x => x.RequestUri!.Query.Contains("page=2")
                ? HttpStatusCode.InternalServerError
                : null
        };
        var sut = GetCrawler("some-key", handler);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 2);

        //all three offers read page 1 completely, none of it is delivered
        result.NewEntries.ShouldBeEmpty();
        result.KnownEntries.ShouldBeEmpty();
        result.Warnings.Count.ShouldBe(3);
        result.Warnings.ShouldAllBe(x => x.Message.Contains("dropped"));
    }

    [Test]
    public async Task CrawlWallpapers_OneOfferFails_OtherOffersStay()
    {
        var handler = new FixtureHandler
        {
            LastPage = 2,
            Fail = x => x.RequestUri!.Query.Contains("purity=010") && x.RequestUri.Query.Contains("page=2")
                ? HttpStatusCode.TooManyRequests
                : null
        };
        var sut = GetCrawler("some-key", handler);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 2);

        //sketchy delivers nothing, sfw and nsfw are complete (what sketchy found before failing does not count as known for them)
        result.NewEntries.Count.ShouldBe(3);
        result.KnownEntries.Count.ShouldBe(9);
        result.Warnings.ShouldHaveSingleItem().Message.ShouldContain("People_Sketchy");
    }

    [Test]
    public async Task CrawlWallpapers_DetailsNotFound_WallSkippedOffersGoOn()
    {
        var handler = new FixtureHandler
        {
            Fail = x => x.RequestUri!.AbsolutePath.EndsWith("/w/vp3pd5")
                ? HttpStatusCode.NotFound
                : null
        };
        var sut = GetCrawler("some-key", handler);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 1);

        result.NewEntries.Select(x => x.FileName).ShouldBe(["wallhaven-lyryk2", "wallhaven-gwvw9d"]);
        result.Warnings.Count.ShouldBe(3);
        result.Warnings.ShouldAllBe(x => x.Message.Contains("vp3pd5"));
    }

    [Test]
    public async Task CrawlWallpapers_DetailsFailTransient_OfferDeliversNothing()
    {
        var handler = new FixtureHandler
        {
            Fail = x => x.RequestUri!.AbsolutePath.EndsWith("/w/lyryk2")
                ? HttpStatusCode.InternalServerError
                : null
        };
        var sut = GetCrawler("some-key", handler);
        sut.AddSourceSiteFilter("Wallhaven");
        sut.AddCategoryFilter("Girls");

        var result = await sut.CrawlWallpapers(1, 1);

        //a skipped wall would be left behind as soon as ten known walls precede it, so the offer fails instead
        result.NewEntries.ShouldBeEmpty();
        result.Warnings.Count.ShouldBe(3);
    }


    private static WallpaperCrawler GetCrawler(string? apiKey, HttpMessageHandler? handler)
    {
        //the stub needs no pacing, rate limiting is tested with the core pipeline
        var services = new ServiceCollection()
            .AddCoreCrawler(x => x.Domains["wallhaven.cc"] = new RateLimit(1000, TimeSpan.FromSeconds(1)))
            .AddWallCrawler(x => x.WallhavenApiKey = apiKey)
            //retries are tested with the core pipeline, here they must not wait
            .Configure<CrawlerRequestOptions>(x => x.Backoff = TimeSpan.FromMilliseconds(1))
            //tests always use the NullLogger
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
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
        //what the search fixture claims as last page, so pages behind the first get asked for
        public int LastPage { get; init; } = 1;
        //a status to answer with instead of the fixture
        public Func<HttpRequestMessage, HttpStatusCode?>? Fail { get; init; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            if (Fail?.Invoke(request) is { } status)
                return new HttpResponseMessage(status);

            var path = request.RequestUri!.AbsolutePath;
            var fixture = path.EndsWith("/search")
                ? GetFixture("search.json")
                : GetFixture($"w_{path[(path.LastIndexOf('/') + 1)..]}.json");
            if (!fixture.Exists)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var json = (await File.ReadAllTextAsync(fixture.FullName, cancellationToken))
                .Replace("\"last_page\": 1", $"\"last_page\": {LastPage}");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }

}
