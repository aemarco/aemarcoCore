using System.Diagnostics.Metrics;

namespace aemarco.Crawler.Wallpaper.Common;

/// <summary>
/// what only the crawl itself can see; a consumer subscribes with AddMeter("aemarco.Crawler*")
/// </summary>
internal static class WallCrawlerMetrics
{

    private static readonly Meter Meter = new("aemarco.Crawler.Wallpaper", typeof(WallCrawlerMetrics).Assembly.GetName().Version?.ToString());

    private static readonly Counter<long> StopReason = Meter.CreateCounter<long>(
        "crawler.stop.reason", description: "Offers which went through, by what ended them: streak (known entries in a row), limit (new entry limit) or end (no more pages)");

    //the crawler's friendly name from its attribute, the name the consumer knows it by
    public static void RecordStop(Type crawlerType, CrawlOffer offer)
    {
        var crawler = CrawlerInfo.FromCrawlerType(crawlerType).FriendlyName;
        var reason = offer.ReachedNewEntryLimit
            ? "limit"
            : offer.ReachedMaximumStreak
                ? "streak"
                : "end";
        StopReason.Add(1,
            new KeyValuePair<string, object?>("crawler", crawler),
            new KeyValuePair<string, object?>("reason", reason));
    }
}
