namespace aemarco.Crawler.Wallpaper.Model;

public class WallCrawlerResult
{
    public int NumberOfCrawlersInvolved { get; init; } = 1;

    public List<WallEntry> NewEntries { get; init; } = [];
    public List<WallEntry> KnownEntries { get; init; } = [];
    public List<AlbumEntry> NewAlbums { get; init; } = [];
    public List<AlbumEntry> KnownAlbums { get; init; } = [];

    public List<Warning> Warnings { get; init; } = [];

}