namespace aemarco.Crawler.Wallpaper.Model;

public class WallCrawlerResult
{
    public int NumberOfCrawlersInvolved { get; set; }

    public List<WallEntry> NewEntries { get; set; } = new();
    public List<WallEntry> KnownEntries { get; set; } = new();
    public List<AlbumEntry> NewAlbums { get; set; } = new();
    public List<AlbumEntry> KnownAlbums { get; set; } = new();
}