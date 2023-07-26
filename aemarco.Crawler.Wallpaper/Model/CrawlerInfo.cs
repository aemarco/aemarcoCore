namespace aemarco.Crawler.Wallpaper.Model;
public record CrawlerInfo(string FriendlyName)
{
    public static CrawlerInfo FromCrawlerType(Type type)
    {
        var attr = type.GetCustomAttribute<WallpaperCrawlerAttribute>();
        return attr is null
            ? throw new Exception($"CrawlerInfo not defined on {type.FullName}")
            : new CrawlerInfo(attr.FriendlyName) { IsAvailable = true };
    }

    internal bool IsAvailable { get; private init; }


    public override string ToString() => FriendlyName;
}
