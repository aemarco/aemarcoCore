using System.Diagnostics;

namespace aemarco.Crawler.Wallpaper;
public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddWallCrawler(this IServiceCollection services)
    {

        services.AddCoreCrawler();

        //crawlers
        List<string> crawlerKeys = [];
        foreach (var crawlerType in typeof(WallpaperCrawlerBasis).Assembly
                     .GetTypes()
                     .Where(x => x.IsSubclassOf(typeof(WallpaperCrawlerBasis))))
        {
            //each crawler must declare a CrawlerAttribute
            if (crawlerType.GetCustomAttribute<CrawlerAttribute>() is null)
                throw new UnreachableException($"CrawlerType {crawlerType.Name} fails to declare CrawlerAttribute");

            crawlerKeys.Add(crawlerType.Name);
            services.AddKeyedTransient(typeof(WallpaperCrawlerBasis), crawlerType.Name, crawlerType);
        }
        services.AddSingleton(new WallCrawlerRegistry([.. crawlerKeys]));


        //internal
        services.AddTransient<IWallCrawlerProvider, WallCrawlerProvider>();


        //public
        services.AddTransient<IWallpaperCrawler, WallpaperCrawler>();
        return services;
    }

}
