namespace aemarco.Crawler.Configuration;

/// <summary>
/// how a crawler request is attempted, for sites as well as for apis (services.Configure&lt;CrawlerRequestOptions&gt;(...))
/// </summary>
public class CrawlerRequestOptions
{
    /// <summary>
    /// attempts per request, the first one included; every attempt takes a permit of the rate limiting
    /// </summary>
    public int MaxAttempts { get; set; } = 5;

    /// <summary>
    /// time one attempt gets on the network, the wait for a permit not included
    /// </summary>
    public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// wait before the second attempt after a 429 or 5xx without a Retry-After, doubled for every further one
    /// </summary>
    public TimeSpan Backoff { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// the longest Retry-After we wait for; a longer one is not retried, the failure goes to the caller
    /// </summary>
    public TimeSpan MaxRetryAfter { get; set; } = TimeSpan.FromMinutes(1);
}
