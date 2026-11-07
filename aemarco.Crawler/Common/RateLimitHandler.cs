namespace aemarco.Crawler.Common;

/// <summary>
/// waits for a permit of the domain, then the random jitter of its limit, before every request
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
        //waits (queued) until the domain's next permit; a lease without one only comes from a full queue, which RateLimit never has
        using var lease = await _rateLimiter.Acquire(request, cancellationToken).ConfigureAwait(false);
        if (!lease.IsAcquired)
            throw new HttpRequestException($"Rate limit queue exceeded for {request.RequestUri}");

        var jitter = _rateLimiter.GetLimit(CrawlerRateLimiter.GetDomain(request.RequestUri)).Jitter;
        if (jitter > TimeSpan.Zero)
            await Task.Delay(Random.Shared.Next((int)jitter.TotalMilliseconds), cancellationToken).ConfigureAwait(false);

        var result = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return result;
    }
}
