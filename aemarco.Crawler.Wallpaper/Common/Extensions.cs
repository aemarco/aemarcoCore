namespace aemarco.Crawler.Wallpaper.Common;

internal static class Extensions
{
    internal static WallpaperCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
    {
        var result = crawlerType.GetCustomAttribute<WallpaperCrawlerAttribute>();
        return result ?? throw new Exception($"PersonCrawler not defined on {crawlerType.FullName}");
    }


    internal static bool IsAvailableCrawler(this Type crawlerType)
    {
        if (!crawlerType.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
            return false;

        if (crawlerType.Namespace is null || crawlerType.Namespace.EndsWith("Obsolete"))
            return false;


        return true;
    }



}