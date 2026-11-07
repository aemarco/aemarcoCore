namespace aemarco.Crawler.Configuration;

/// <summary>
/// options for the one rate limiting all crawler requests go through, one limit per domain (host without "www.");
/// packages and consumers contribute with services.Configure&lt;CrawlerRateLimits&gt;(...)
/// </summary>
public class CrawlerRateLimits
{
    /// <summary>
    /// applies to every domain without an own entry in Domains: one request per second, plus up to 0.5s
    /// </summary>
    public RateLimit Default { get; set; } = new(60, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// per domain, starting with the known paces deviating from the default;
    /// defaults here, not in a Configure, so every consumer Configure comes after them, regardless of the call order
    /// </summary>
    public Dictionary<string, RateLimit> Domains { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        //wallpaperscraft was crawled with 1-2.5s between its pages
        ["wallpaperscraft.com"] = new RateLimit(60, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(1500)),
        //the wallhaven api allows 45 calls per minute, evenly spread with some headroom, no jitter for an api
        ["wallhaven.cc"] = new RateLimit(40, TimeSpan.FromMinutes(1))
    };
}

/// <summary>
/// Permits calls per Window, spread evenly (one every Window / Permits, no bursts),
/// each call waits a random 0-Jitter on top, so the pacing does not look mechanical
/// </summary>
public record RateLimit(int Permits, TimeSpan Window, TimeSpan Jitter = default)
{
    //token bucket of size 1: the next permit comes one interval after the last, requests beyond wait in a queue
    internal TokenBucketRateLimiterOptions ToLimiterOptions() => new()
    {
        TokenLimit = 1,
        TokensPerPeriod = 1,
        ReplenishmentPeriod = Window / Permits,
        QueueLimit = int.MaxValue,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        AutoReplenishment = true
    };
}
