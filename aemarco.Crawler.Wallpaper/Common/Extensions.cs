using System;
using aemarcoCommons.Extensions.AttributeExtensions;

namespace aemarco.Crawler.Wallpaper.Common
{
    public static class Extensions
    {
        public static WallpaperCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
        {
            var attr = crawlerType.GetAttribute<WallpaperCrawlerAttribute>();
            return attr;
        }
    }
}
