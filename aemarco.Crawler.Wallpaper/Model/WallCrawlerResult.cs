namespace aemarco.Crawler.Wallpaper.Model;

public class WallCrawlerResult
{
    public int NumberOfCrawlersInvolved { get; init; } = 1;

    public List<WallEntry> NewEntries { get; init; } = new();
    public List<WallEntry> KnownEntries { get; init; } = new();
    public List<AlbumEntry> NewAlbums { get; init; } = new();
    public List<AlbumEntry> KnownAlbums { get; init; } = new();

    public List<Warning> Warnings { get; init; } = new();

}