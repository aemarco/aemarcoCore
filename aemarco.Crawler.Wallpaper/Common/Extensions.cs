using System;
using System.Reflection;


namespace aemarco.Crawler.Wallpaper.Common
{
    public static class Extensions
    {
        public static WallpaperCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
        {
            return crawlerType.GetCustomAttribute<WallpaperCrawlerAttribute>();

        }


        public static bool IsAvailableCrawler(this Type crawlerType)
        {
            if (!crawlerType.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
                return false;

            if (crawlerType.Namespace is null || crawlerType.Namespace.EndsWith("Obsolete"))
                return false;


            return true;
        }



    }
}
