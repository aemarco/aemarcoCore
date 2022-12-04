namespace aemarco.Crawler.Wallpaper.Model;

public class AlbumEntry
{
    public AlbumEntry(string name)
    {
        Name = name;
        Entries = new List<WallEntry>();
    }

    public string Name { get; }
    public List<WallEntry> Entries { get; }


    internal bool HasNewEntries { get; set; }


}