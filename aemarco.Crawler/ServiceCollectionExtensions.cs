using aemarco.Crawler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace aemarco.Crawler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreCrawler(this IServiceCollection services)
    {

        services.AddSingleton<ICountryService, CountryService>();

        return services;
    }

}
