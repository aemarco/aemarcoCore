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
    }
}
