using aemarco.Crawler;
using aemarco.Crawler.Clients;
using aemarco.Crawler.Common;
using aemarco.Crawler.Configuration;
using aemarco.Crawler.Contracts.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace aemarco.CrawlerTests.Common;

//the retries and the timeout per attempt of the crawler pipeline, the network replaced by a stub
internal class CrawlerRetryTests
{

    [Test]
    public async Task Navigate_TooManyRequestsWithRetryAfter_TriesAgain()
    {
        var (sut, handler) = GetClient(attempt => attempt == 1
            ? Answer(HttpStatusCode.TooManyRequests, retryAfter: TimeSpan.Zero)
            : Answer(HttpStatusCode.OK));

        var result = await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        result.ShouldNotBeNull();
        handler.Attempts.ShouldBe(2);
    }

    [Test]
    public async Task Navigate_ServerErrorsWithoutRetryAfter_TriesAgainWithBackoff()
    {
        var (sut, handler) = GetClient(attempt => attempt <= 2
            ? Answer(HttpStatusCode.ServiceUnavailable)
            : Answer(HttpStatusCode.OK));

        var result = await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        result.ShouldNotBeNull();
        handler.Attempts.ShouldBe(3);
    }

    [Test]
    public async Task Navigate_ServerErrorStays_ThrowsAfterMaxAttempts()
    {
        var (sut, handler) = GetClient(_ => Answer(HttpStatusCode.InternalServerError));

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        var ex = await request.ShouldThrowAsync<HttpRequestException>();
        ex.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        handler.Attempts.ShouldBe(3);
    }

    [Test]
    public async Task Navigate_RetryAfterBeyondMax_NotRetried()
    {
        var (sut, handler) = GetClient(_ => Answer(HttpStatusCode.TooManyRequests, retryAfter: TimeSpan.FromHours(1)));

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        var ex = await request.ShouldThrowAsync<HttpRequestException>();
        ex.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
        handler.Attempts.ShouldBe(1);
    }

    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.Forbidden)]
    public async Task Navigate_NoRetryStatus_AnsweredOnce(HttpStatusCode status)
    {
        var (sut, handler) = GetClient(_ => Answer(status));

        try
        {
            await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);
        }
        catch (HttpRequestException)
        {
            //a 403 throws, a 404 delivers its page, neither is tried again
        }

        handler.Attempts.ShouldBe(1);
    }

    [Test]
    public async Task Navigate_Retries_TakeTheirPermit()
    {
        var (sut, _) = GetClient(
            attempt => attempt <= 2
                ? Answer(HttpStatusCode.TooManyRequests, retryAfter: TimeSpan.Zero)
                : Answer(HttpStatusCode.OK),
            limits: x => x.Default = new RateLimit(10, TimeSpan.FromSeconds(1)));
        var sw = Stopwatch.StartNew();

        await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        //10 per second, three attempts: the first goes at once, each retry waits an interval
        sw.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(180));
    }

    [Test]
    public async Task Navigate_AttemptsTimeOut_TimeoutExceptionAfterMaxAttempts()
    {
        var (sut, handler) = GetClient(_ => Hang());

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        //not an OperationCanceledException, the caller did not cancel
        await request.ShouldThrowAsync<TimeoutException>();
        handler.Attempts.ShouldBe(3);
    }

    [Test]
    public async Task Navigate_TimeoutThenAnswer_TriesAgain()
    {
        var (sut, handler) = GetClient(attempt => attempt == 1
            ? Hang()
            : Answer(HttpStatusCode.OK));

        var result = await sut.Navigate(new PageUri(new Uri("https://example.com/")), CancellationToken.None);

        result.ShouldNotBeNull();
        handler.Attempts.ShouldBe(2);
    }

    [Test]
    public async Task Navigate_CallerCancels_CancellationNotRetried()
    {
        var (sut, handler) = GetClient(_ => Hang());
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        var request = sut.Navigate(new PageUri(new Uri("https://example.com/")), cts.Token);

        await request.ShouldThrowAsync<OperationCanceledException>();
        handler.Attempts.ShouldBe(1);
    }


    private static Func<CancellationToken, Task<HttpResponseMessage>> Answer(HttpStatusCode status, TimeSpan? retryAfter = null) =>
        _ =>
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent("<html><body><p>stub page</p></body></html>")
            };
            if (retryAfter is { } delay)
                response.Headers.RetryAfter = new RetryConditionHeaderValue(delay);
            return Task.FromResult(response);
        };

    //never answers, only the timeout of the attempt (or the caller) ends it
    private static Func<CancellationToken, Task<HttpResponseMessage>> Hang() =>
        async token =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return new HttpResponseMessage(HttpStatusCode.OK);
        };

    private static (IHtmlClient Client, AttemptHandler Handler) GetClient(
        Func<int, Func<CancellationToken, Task<HttpResponseMessage>>> respond,
        Action<CrawlerRateLimits>? limits = null)
    {
        var handler = new AttemptHandler(respond);
        var services = new ServiceCollection()
            .AddCoreCrawler(limits ?? (x => x.Default = new RateLimit(1000, TimeSpan.FromSeconds(1))))
            //tests always use the NullLogger
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .Configure<CrawlerRequestOptions>(x =>
            {
                x.MaxAttempts = 3;
                x.AttemptTimeout = TimeSpan.FromMilliseconds(100);
                x.Backoff = TimeSpan.FromMilliseconds(1);
                x.MaxRetryAfter = TimeSpan.FromSeconds(5);
            });
        services
            .AddHttpClient<IHtmlClient, HtmlClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var client = services
            .BuildServiceProvider()
            .GetRequiredService<IHtmlClient>();
        return (client, handler);
    }

    private class AttemptHandler : HttpMessageHandler
    {
        private readonly Func<int, Func<CancellationToken, Task<HttpResponseMessage>>> _respond;
        public AttemptHandler(
            Func<int, Func<CancellationToken, Task<HttpResponseMessage>>> respond)
        {
            _respond = respond;
        }

        private int _attempts;
        public int Attempts => _attempts;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var attempt = Interlocked.Increment(ref _attempts);
            var result = await _respond(attempt)(cancellationToken);
            return result;
        }
    }

}
