using aemarco.Crawler.Clients;
using aemarco.Crawler.Services;

namespace aemarco.Crawler;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// the crawler packages call this themselves; the consumer calls it once more with configure for the rate limits
    /// (on top of the defaults of CrawlerRateLimits, whatever the call order)
    /// </summary>
    public static IServiceCollection AddCoreCrawler(
        this IServiceCollection services,
        Action<CrawlerRateLimits>? configure = null)
    {
        //every crawler package calls this, so everything registers only once
        services.TryAddSingleton<ICountryService, CountryService>();

        //rate limits from the consumer
        if (configure is not null)
            services.Configure(configure);

        //clients
        _ = services.AddHtmlClient();

        return services;
    }

    /// <summary>
    /// attaches the crawler request pipeline to an HttpClient, e.g. the one of an api:
    /// retries outside (timeouts, 408/429/5xx), then the rate limiting (so every attempt takes its permit),
    /// then the timeout of one attempt on the network
    /// </summary>
    public static IHttpClientBuilder AddCrawlerPipeline(this IHttpClientBuilder builder) =>
        builder
            //the attempts time out themselves, the wait for a permit is not part of it
            .ConfigureHttpClient(x => x.Timeout = Timeout.InfiniteTimeSpan)
            .AddHttpMessageHandler<RetryHandler>()
            .AddHttpMessageHandler<RateLimitHandler>()
            .AddHttpMessageHandler<AttemptTimeoutHandler>();


    private static IServiceCollection AddHtmlClient(this IServiceCollection services)
    {

        //the one rate limiting for all crawler requests, limits per domain from CrawlerRateLimits
        services.AddOptions<CrawlerRateLimits>();
        services.AddOptions<CrawlerRequestOptions>();
        services.TryAddSingleton<CrawlerRateLimiter>();
        services.TryAddTransient<RetryHandler>();
        services.TryAddTransient<RateLimitHandler>();
        services.TryAddTransient<AttemptTimeoutHandler>();

        //AddHttpClient appends its handlers on every call, a second registration would rate limit and retry twice
        if (services.Any(x => x.ServiceType == typeof(IHtmlClient)))
            return services;

        services
            .AddHttpClient<IHtmlClient, HtmlClient>()
            //like HtmlWeb: decompressed, no cookies kept
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                UseCookies = false
            })
            .AddCrawlerPipeline();
        return services;
    }

}
