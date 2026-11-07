using Microsoft.Extensions.Options;

namespace aemarco.Crawler.Common;

/// <summary>
/// singleton, the one rate limiting all crawler requests go through, one token bucket per domain
/// </summary>
internal class CrawlerRateLimiter : IDisposable
{

    private readonly CrawlerRateLimits _limits;
    private readonly PartitionedRateLimiter<HttpRequestMessage> _limiter;
    public CrawlerRateLimiter(
        IOptions<CrawlerRateLimits> options)
    {
        _limits = options.Value;
        _limiter = PartitionedRateLimiter.Create<HttpRequestMessage, string>(request =>
        {
            var domain = GetDomain(request.RequestUri);
            return RateLimitPartition.GetTokenBucketLimiter(domain, x => GetLimit(x).ToLimiterOptions());
        });
    }

    /// <summary>
    /// the host without "www.", so www.example.com and example.com share one limit
    /// </summary>
    public static string GetDomain(Uri? uri)
    {
        var host = uri?.Host.ToLowerInvariant() ?? string.Empty;
        return host.StartsWith("www.")
            ? host[4..]
            : host;
    }

    public RateLimit GetLimit(string domain) =>
        _limits.Domains.TryGetValue(domain, out var limit)
            ? limit
            : _limits.Default;

    public async ValueTask<RateLimitLease> Acquire(HttpRequestMessage request, CancellationToken token)
    {
        var result = await _limiter.AcquireAsync(request, 1, token).ConfigureAwait(false);
        return result;
    }

    public void Dispose() => _limiter.Dispose();
}
