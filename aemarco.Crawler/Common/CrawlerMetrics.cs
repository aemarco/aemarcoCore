using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace aemarco.Crawler.Common;

/// <summary>
/// what only the request pipeline can see; a consumer subscribes with AddMeter("aemarco.Crawler*"),
/// without a listener the instruments cost next to nothing
/// </summary>
internal static class CrawlerMetrics
{

    public const string MeterName = "aemarco.Crawler";
    private static readonly Meter Meter = new(MeterName, typeof(CrawlerMetrics).Assembly.GetName().Version?.ToString());

    /// <summary>
    /// how long the rate limiting held a request back (jitter and the wait in line at the domain's gate)
    /// </summary>
    public static readonly Histogram<double> RateLimitWait = Meter.CreateHistogram<double>(
        "crawler.ratelimit.wait", "s", "Time a request waited for its turn (jitter and gate) before it was sent");

    /// <summary>
    /// the network time of one attempt, without the wait for a permit
    /// </summary>
    public static readonly Histogram<double> AttemptDuration = Meter.CreateHistogram<double>(
        "crawler.http.attempt.duration", "s", "Time of one attempt on the network until the response headers, without the rate limiting");

    /// <summary>
    /// everything a caller waits for: permits, attempts and the backoff between them
    /// </summary>
    public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "crawler.http.request.duration", "s", "Time of a request from the caller's view, including rate limiting and retries");


    public static void RecordWait(long startTimestamp, string domain)
    {
        if (RateLimitWait.Enabled)
            RateLimitWait.Record(Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds, new KeyValuePair<string, object?>("domain", domain));
    }

    /// <summary>
    /// the status code when there was a response, the exception type when there was not
    /// </summary>
    public static void RecordHttp(Histogram<double> histogram, long startTimestamp, string domain, HttpResponseMessage? response, Exception? error)
    {
        if (!histogram.Enabled)
            return;

        var tags = new TagList { { "domain", domain } };
        if (response is not null)
            tags.Add("http.response.status_code", (int)response.StatusCode);
        if (error is not null)
            tags.Add("error.type", error.GetType().Name);
        histogram.Record(Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds, tags);
    }
}
