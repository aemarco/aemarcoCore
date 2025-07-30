using Microsoft.Extensions.DependencyInjection;

namespace aemarco.Crawler.Wallpaper;
public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddWallCrawler(this IServiceCollection services)
    {

        services.AddCoreCrawler();





        services.AddTransient<IWallpaperCrawler, WallpaperCrawler>();

        return services;
    }

}
