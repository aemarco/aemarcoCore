namespace aemarco.Crawler.Model;

/// <summary>
/// at most Permits calls in any span of Window (sliding window, requests beyond wait in a queue)
/// </summary>
public record RateLimit(int Permits, TimeSpan Window)
{
    //the window counts permits per segment, the oldest segment frees its permits as the window slides on;
    //12 segments = 5s steps for a 1 minute window, so a permit comes back at most one step late (never early)
    public RateLimiter CreateLimiter() =>
        new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = Permits,
            Window = Window,
            SegmentsPerWindow = 12,
            QueueLimit = int.MaxValue,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
}
