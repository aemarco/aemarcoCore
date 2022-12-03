using System;

namespace aemarco.Crawler.Wallpaper.Common
{
    public class WallpaperCrawlerAttribute : Attribute
    {
        public WallpaperCrawlerAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
        public string FriendlyName { get; }

    }
}
