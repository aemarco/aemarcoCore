using System;
using System.Security.Cryptography;

namespace aemarco.Crawler.Wallpaper.Common
{
    public class WallpaperCrawlerAttribute : Attribute
    {
        public WallpaperCrawlerAttribute(string friendlyName, bool isEnabled)
        {
            FriendlyName = friendlyName;
            IsEnabled = isEnabled;

        }
        public string FriendlyName { get; }
        public bool IsEnabled { get; }
    }
}
