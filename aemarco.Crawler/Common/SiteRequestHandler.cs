namespace aemarco.Crawler.Common;

/// <summary>
/// treatment for crawled sites only (not for apis): retries an attempt that timed out;
/// sits inside the rate limiting, so the attempt timeout covers the network only, not the wait for a permit
/// </summary>
internal class SiteRequestHandler : DelegatingHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;
        var attemptTimeout = TimeSpan.FromSeconds(30);

        for (var attempt = 1; ; attempt++)
        {
            using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            attemptCts.CancelAfter(attemptTimeout);
            try
            {
                var result = await base.SendAsync(request, attemptCts.Token).ConfigureAwait(false);
                return result;
            }
            catch (OperationCanceledException) when (
                !cancellationToken.IsCancellationRequested &&
                attempt < maxAttempts)
            {
                //the attempt timed out, not the caller, so we try again
            }
        }
    }
}
