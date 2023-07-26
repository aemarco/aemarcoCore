namespace aemarco.Crawler.Wallpaper.Common;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class WallpaperCrawlerAttribute : Attribute
{
    public WallpaperCrawlerAttribute(string friendlyName)
    {
        FriendlyName = friendlyName;
    }
    public string FriendlyName { get; }

}