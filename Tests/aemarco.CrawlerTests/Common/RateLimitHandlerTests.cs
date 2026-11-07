using aemarco.Crawler.Common;
using aemarco.Crawler.Model;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace aemarco.CrawlerTests.Common;

internal class RateLimitHandlerTests
{

    [Test]
    public async Task SendAsync_WithinLimit_PassesImmediately()
    {
        using var client = GetClient(new RateLimit(2, TimeSpan.FromSeconds(10)));
        var sw = Stopwatch.StartNew();

        await client.GetAsync("https://example.com/1");
        await client.GetAsync("https://example.com/2");

        sw.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task SendAsync_OverLimit_WaitsForWindow()
    {
        using var client = GetClient(new RateLimit(2, TimeSpan.FromMilliseconds(600)));
        await client.GetAsync("https://example.com/1");
        await client.GetAsync("https://example.com/2");
        var sw = Stopwatch.StartNew();

        await client.GetAsync("https://example.com/3");

        //the third permit frees up once the first two slide out of the window
        sw.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(300));
        sw.Elapsed.Dump();
    }

    [Test]
    public async Task SendAsync_CancelledWhileWaiting_Throws()
    {
        using var client = GetClient(new RateLimit(1, TimeSpan.FromHours(1)));
        await client.GetAsync("https://example.com/1");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        var request = client.GetAsync("https://example.com/2", cts.Token);

        await request.ShouldThrowAsync<OperationCanceledException>();
    }


    private static HttpClient GetClient(RateLimit rateLimit)
    {
        var handler = new RateLimitHandler(rateLimit.CreateLimiter())
        {
            InnerHandler = new OkHandler()
        };
        var result = new HttpClient(handler);
        return result;
    }

    private class OkHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }

}
