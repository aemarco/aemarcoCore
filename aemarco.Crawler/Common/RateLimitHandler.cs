namespace aemarco.Crawler.Common;

/// <summary>
/// waits for a permit of the given limiter before every request of the HttpClient it is attached to
/// </summary>
public class RateLimitHandler : DelegatingHandler
{

    private readonly RateLimiter _limiter;
    public RateLimitHandler(
        RateLimiter limiter)
    {
        _limiter = limiter;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var lease = await _limiter.AcquireAsync(1, cancellationToken).ConfigureAwait(false);
        if (!lease.IsAcquired)
            throw new HttpRequestException($"Rate limit queue exceeded for {request.RequestUri}");

        var result = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return result;
    }
}
