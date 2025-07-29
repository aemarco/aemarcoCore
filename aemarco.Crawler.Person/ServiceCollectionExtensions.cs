using System.Diagnostics;

namespace aemarco.Crawler.Person;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddPersonCrawler(this IServiceCollection services)
    {

        services.AddCoreCrawler();

        //crawlers
        Dictionary<string, CrawlerAttribute> crawlerAttributes = [];
        foreach (var crawlerType in typeof(ISiteCrawler).Assembly
                     .GetTypes()
                     .Where(x =>
                         x.IsAssignableTo(typeof(ISiteCrawler)) &&
                         x is { IsAbstract: false, IsClass: true }))
        {
            //each crawler must declare a CrawlerAttribute
            if (crawlerType.GetCustomAttribute<CrawlerAttribute>() is not { } info)
                throw new UnreachableException($"CrawlerType {crawlerType.Name} fails to declare CrawlerAttribute");

            crawlerAttributes.Add(crawlerType.Name, info);
            services.AddKeyedTransient(typeof(ISiteCrawler), crawlerType.Name, crawlerType);
        }
        services.AddSingleton(crawlerAttributes);


        //internal
        services.AddTransient<ISiteCrawlerProvider, SiteCrawlerProvider>();


        //public
        services.AddTransient<IPersonCrawler, PersonCrawler>();
        return services;

    }

}