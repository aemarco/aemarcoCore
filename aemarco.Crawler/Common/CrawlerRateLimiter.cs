using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace aemarco.Crawler.Common;

/// <summary>
/// singleton, the one rate limiting all crawler requests go through: per domain a gate which knows the next
/// time a request may be sent, one interval (Window / Permits) after the last one. Requests wait in line for it.
/// </summary>
internal class CrawlerRateLimiter
{

    private readonly CrawlerRateLimits _limits;
    private readonly ConcurrentDictionary<string, DomainGate> _gates = new();
    public CrawlerRateLimiter(
        IOptions<CrawlerRateLimits> options)
    {
        _limits = options.Value;
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

    /// <summary>
    /// returns when it is the request's turn on its domain, so it can be sent right away
    /// </summary>
    public async Task WaitForTurn(HttpRequestMessage request, CancellationToken token)
    {
        var domain = GetDomain(request.RequestUri);
        var gate = _gates.GetOrAdd(domain, _ => new DomainGate());
        await gate.WaitForTurn(GetLimit(domain).Interval, token).ConfigureAwait(false);
    }


    private sealed class DomainGate
    {

        private readonly SemaphoreSlim _line = new(1, 1);
        //Stopwatch timestamp of the next allowed send, the default lies in the past
        private long _nextAllowed;

        public async Task WaitForTurn(TimeSpan interval, CancellationToken token)
        {
            await _line.WaitAsync(token).ConfigureAwait(false);
            try
            {
                var wait = Stopwatch.GetElapsedTime(Stopwatch.GetTimestamp(), _nextAllowed);
                if (wait > TimeSpan.Zero)
                    await Task.Delay(wait, token).ConfigureAwait(false);

                //the next one may go one interval after this one
                _nextAllowed = Stopwatch.GetTimestamp() + (long)(interval.TotalSeconds * Stopwatch.Frequency);
            }
            finally
            {
                _line.Release();
            }
        }
    }
}
