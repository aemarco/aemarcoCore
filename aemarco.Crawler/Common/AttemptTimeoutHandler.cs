using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace aemarco.Crawler.Common;

/// <summary>
/// limits the time of one attempt on the network; it sits inside the rate limiting,
/// so the timeout does not count the wait for a permit. A timeout is a TimeoutException (not a cancellation,
/// the caller did not cancel), which the RetryHandler tries again.
/// </summary>
internal class AttemptTimeoutHandler : DelegatingHandler
{

    private readonly CrawlerRequestOptions _options;
    public AttemptTimeoutHandler(
        IOptions<CrawlerRequestOptions> options)
    {
        _options = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        attemptCts.CancelAfter(_options.AttemptTimeout);
        var start = Stopwatch.GetTimestamp();
        HttpResponseMessage? result = null;
        Exception? error = null;
        try
        {
            result = await base.SendAsync(request, attemptCts.Token).ConfigureAwait(false);
            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            error = new TimeoutException($"No answer from {request.RequestUri} within {_options.AttemptTimeout}");
            throw error;
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            CrawlerMetrics.RecordHttp(CrawlerMetrics.AttemptDuration, start,
                CrawlerRateLimiter.GetDomain(request.RequestUri), result, error);
        }
    }
}
