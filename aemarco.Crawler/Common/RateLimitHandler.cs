namespace aemarco.Crawler.Common;

/// <summary>
/// waits the random jitter of the domain's limit, then its turn at the domain's gate, before every request;
/// the gate lets two sends go at least one interval apart, whatever the jitter is
/// </summary>
internal class RateLimitHandler : DelegatingHandler
{

    private readonly CrawlerRateLimiter _rateLimiter;
    public RateLimitHandler(
        CrawlerRateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var jitter = _rateLimiter.GetLimit(CrawlerRateLimiter.GetDomain(request.RequestUri)).Jitter;
        if (jitter > TimeSpan.Zero)
            await Task.Delay(Random.Shared.Next((int)jitter.TotalMilliseconds), cancellationToken).ConfigureAwait(false);

        //waits in line until it is this request's turn on the domain
        await _rateLimiter.WaitForTurn(request, cancellationToken).ConfigureAwait(false);

        var result = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return result;
    }
}
