using System.Collections.Generic;

namespace aemarco.Crawler.Wallpaper.Model
{
    public class WallCrawlerResult
    {
        public int NumberOfCrawlersInvolved { get; set; }

        public List<WallEntry> NewEntries { get; set; } = new List<WallEntry>();
        public List<WallEntry> KnownEntries { get; set; } = new List<WallEntry>();
        public List<AlbumEntry> NewAlbums { get; set; } = new List<AlbumEntry>();
        public List<AlbumEntry> KnownAlbums { get; set; } = new List<AlbumEntry>();
    }
}
