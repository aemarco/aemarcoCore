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
    /// attaches the crawler rate limiting to an HttpClient, e.g. the one of an api
    /// </summary>
    public static IHttpClientBuilder AddCrawlerRateLimit(this IHttpClientBuilder builder) =>
        builder.AddHttpMessageHandler<RateLimitHandler>();


    private static IServiceCollection AddHtmlClient(this IServiceCollection services)
    {

        //the one rate limiting for all crawler requests, limits per domain from CrawlerRateLimits
        services.AddOptions<CrawlerRateLimits>();
        services.TryAddSingleton<CrawlerRateLimiter>();
        services.TryAddTransient<RateLimitHandler>();
        services.TryAddTransient<SiteRequestHandler>();

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
            //outside: one permit per page load; inside: the site treatment per network attempt
            .AddCrawlerRateLimit()
            .AddHttpMessageHandler<SiteRequestHandler>();
        return services;
    }

}
