using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace aemarco.Crawler.Common;

/// <summary>
/// outermost: tries a request again when an attempt timed out or the server answered 408, 429 or a 5xx
/// (waiting for its Retry-After, otherwise a doubling backoff). Every attempt goes through the rate limiting
/// again, so retries take their permit like any request. The last attempt delivers what it got:
/// its response (the client turns the status into an exception) or its TimeoutException.
/// </summary>
internal class RetryHandler : DelegatingHandler
{

    private static readonly HashSet<HttpStatusCode> RetryStatus =
    [
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    ];

    private readonly CrawlerRequestOptions _options;
    private readonly ILogger<RetryHandler> _logger;
    public RetryHandler(
        IOptions<CrawlerRequestOptions> options,
        ILogger<RetryHandler> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            var isLast = attempt >= _options.MaxAttempts;

            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException) when (!isLast)
            {
                _logger.LogWarning("Request to {uri} timed out on attempt {attempt} of {maxAttempts}, trying again",
                    request.RequestUri, attempt, _options.MaxAttempts);
                continue;
            }

            if (isLast || !RetryStatus.Contains(response.StatusCode))
                return response;
            if (GetDelay(response, attempt) is not { } delay)
                return response;

            _logger.LogWarning("Request to {uri} answered {status} on attempt {attempt} of {maxAttempts}, trying again in {delay}",
                request.RequestUri, (int)response.StatusCode, attempt, _options.MaxAttempts, delay);
            response.Dispose();
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// null when the server wants us to wait longer than we are willing to
    /// </summary>
    private TimeSpan? GetDelay(HttpResponseMessage response, int attempt)
    {
        var retryAfter = response.Headers.RetryAfter;
        var requested = retryAfter?.Delta ?? (retryAfter?.Date - DateTimeOffset.UtcNow);
        if (requested > _options.MaxRetryAfter)
            return null;

        var result = requested ?? _options.Backoff * Math.Pow(2, attempt - 1);
        return result < TimeSpan.Zero
            ? TimeSpan.Zero
            : result;
    }
}
