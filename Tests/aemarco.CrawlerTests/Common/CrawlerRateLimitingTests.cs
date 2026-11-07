using aemarco.Crawler;
using aemarco.Crawler.Clients;
using aemarco.Crawler.Common;
using aemarco.Crawler.Configuration;
using aemarco.Crawler.Contracts.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace aemarco.CrawlerTests.Common;

//the whole site pipeline (rate limiting, site treatment, html client) from AddCoreCrawler, the network replaced by a stub
internal class CrawlerRateLimitingTests
{

    [Test]
    public async Task Navigate_SameDomain_EvenlySpaced()
    {
        var (sut, _) = GetClient(x => x.Default = new RateLimit(10, TimeSpan.FromSeconds(1)));
        var sw = Stopwatch.StartNew();

        await sut.Navigate(new PageUri(new Uri("https://example.com/1")), CancellationToken.None);
        await sut.Navigate(new PageUri(new Uri("https://example.com/2")), CancellationToken.None);
        await sut.Navigate(new PageUri(new Uri("https://example.com/3")), CancellationToken.None);

        //10 per second, so one every 100ms: the first goes at once, the next two wait an interval each
        sw.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(180));
        sw.Elapsed.Dump();
    }

    [Test]
    public async Task Navigate_LargeJitter_SendsNeverCloserThanTheInterval()
    {
        //100ms interval, jitter up to 300ms: the jitter must not let two sends come closer than the interval
        var (sut, handler) = GetClient(x => x.Default = new RateLimit(10, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(300)));

        for (var i = 0; i < 10; i++)
            await sut.Navigate(new PageUri(new Uri($"https://example.com/{i}")), CancellationToken.None);

        var gaps = handler.Timestamps
            .Zip(handler.Timestamps.Skip(1), (first, second) => Stopwatch.GetElapsedTime(first, second))
            .ToList();
        //a little below 100ms for the timer's granularity
        gaps.ShouldAllBe(x => x >= TimeSpan.FromMilliseconds(80));
        gaps.Dump();
    }

    [Test]
    public async Task Navigate_DifferentDomains_NotHeldBack()
    {
        var (sut, _) = GetClient(x => x.Default = new RateLimit(1, TimeSpan.FromHours(1)));
        var sw = Stopwatch.StartNew();

        await sut.Navigate(new PageUri(new Uri("https://first.com/")), CancellationToken.None);
        await sut.Navigate(new PageUri(new Uri("https://second.com/")), CancellationToken.None);

        sw.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task Navigate_WwwAndBareDomain_ShareTheLimit()
    {
        var (sut, _) = GetClient(x => x.Default = new RateLimit(1, TimeSpan.FromHours(1)));
        await sut.Navigate(new PageUri(new Uri("https://www.example.com/")), CancellationToken.None);
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/")), cts.Token);

        await request.ShouldThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task Navigate_DomainEntry_OverridesDefault()
    {
        var (sut, _) = GetClient(x =>
        {
            x.Default = new RateLimit(1, TimeSpan.FromHours(1));
            x.Domains["example.com"] = new RateLimit(100, TimeSpan.FromSeconds(1));
        });
        var sw = Stopwatch.StartNew();

        await sut.Navigate(new PageUri(new Uri("https://example.com/1")), CancellationToken.None);
        await sut.Navigate(new PageUri(new Uri("https://example.com/2")), CancellationToken.None);

        sw.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task Navigate_NotFound_StillDeliversThePage()
    {
        var (sut, handler) = GetClient(x => x.Default = new RateLimit(100, TimeSpan.FromSeconds(1)));
        handler.Status = HttpStatusCode.NotFound;

        var result = await sut.Navigate(new PageUri(new Uri("https://example.com/gone")), CancellationToken.None);

        result.ShouldNotBeNull().FindNode("//p")?.GetText().ShouldBe("stub page");
    }

    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.TooManyRequests)]
    [TestCase(HttpStatusCode.InternalServerError)]
    [TestCase(HttpStatusCode.ServiceUnavailable)]
    public async Task Navigate_ErrorStatus_ThrowsWithStatus(HttpStatusCode status)
    {
        var (sut, handler) = GetClient(x => x.Default = new RateLimit(100, TimeSpan.FromSeconds(1)));
        handler.Status = status;

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/blocked")), CancellationToken.None);

        var ex = await request.ShouldThrowAsync<HttpRequestException>();
        ex.StatusCode.ShouldBe(status);
    }

    [Test]
    public async Task Navigate_NullPageUri_ReturnsNullWithoutRequest()
    {
        var (sut, handler) = GetClient(x => x.Default = new RateLimit(100, TimeSpan.FromSeconds(1)));

        var result = await sut.Navigate(null, CancellationToken.None);

        result.ShouldBeNull();
        handler.Requests.ShouldBeEmpty();
    }

    [Test]
    public async Task Navigate_Site_SendsTheKnownUserAgent()
    {
        var (sut, handler) = GetClient(x => x.Default = new RateLimit(100, TimeSpan.FromSeconds(1)));

        await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        handler.Requests.Single().Headers.UserAgent.ToString().ShouldContain("Firefox");
    }

    [Test]
    public async Task AddCoreCrawler_CalledTwice_PipelineOnlyOnce()
    {
        //every crawler package calls AddCoreCrawler; a doubled pipeline would need two permits per request
        var handler = new StubHandler();
        var services = new ServiceCollection()
            .AddCoreCrawler(x => x.Default = new RateLimit(1, TimeSpan.FromHours(1)))
            .AddCoreCrawler()
            //tests always use the NullLogger
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services
            .AddHttpClient<IHtmlClient, HtmlClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var sut = services
            .BuildServiceProvider()
            .GetRequiredService<IHtmlClient>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        await sut.Navigate(new PageUri(new Uri("https://example.com/")), cts.Token);

        handler.Requests.Count.ShouldBe(1);
    }

    [Test]
    public void AddCoreCrawler_ConsumerAction_WinsOverDefaultsRegardlessOfOrder()
    {
        //packages call AddCoreCrawler before and after the consumer's call, that must not bring the defaults back
        var services = new ServiceCollection()
            .AddCoreCrawler()
            .AddCoreCrawler(x => x.Domains["wallhaven.cc"] = new RateLimit(10, TimeSpan.FromMinutes(1)))
            .AddCoreCrawler();

        var result = services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<CrawlerRateLimits>>()
            .Value;

        result.Domains["wallhaven.cc"].ShouldBe(new RateLimit(10, TimeSpan.FromMinutes(1)));
        result.Domains["wallpaperscraft.com"].Permits.ShouldBe(60);
    }


    private static (IHtmlClient Client, StubHandler Handler) GetClient(Action<CrawlerRateLimits> configure)
    {
        var handler = new StubHandler();
        var services = new ServiceCollection()
            .AddCoreCrawler(configure)
            //retries are tested apart, here they must not wait
            .Configure<CrawlerRequestOptions>(x => x.Backoff = TimeSpan.FromMilliseconds(1))
            //tests always use the NullLogger
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services
            .AddHttpClient<IHtmlClient, HtmlClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var client = services
            .BuildServiceProvider()
            .GetRequiredService<IHtmlClient>();
        return (client, handler);
    }

    private class StubHandler : HttpMessageHandler
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public List<HttpRequestMessage> Requests { get; } = [];
        public List<long> Timestamps { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            Timestamps.Add(Stopwatch.GetTimestamp());
            return Task.FromResult(new HttpResponseMessage(Status)
            {
                Content = new StringContent("<html><body><p>stub page</p></body></html>")
            });
        }
    }

}
