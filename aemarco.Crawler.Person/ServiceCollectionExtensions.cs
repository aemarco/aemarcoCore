using Microsoft.Extensions.DependencyInjection;

namespace aemarco.Crawler.Person;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersonCrawler(this IServiceCollection services)
    {
        services.AddTransient<PersonCrawler>();
        return services;
    }
}