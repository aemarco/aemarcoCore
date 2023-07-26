// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace aemarco.Crawler.Wallpaper.Model;

public class WallEntry
{

    internal WallEntry(
        string url,
        string? thumbnailUrl,
        string fileName,
        string extension,
        ContentCategory contentCategory,
        string siteCategory,
        List<string> tags,
        string? albumName)
    {
        Url = url;
        ThumbnailUrl = thumbnailUrl;
        FileName = fileName;
        Extension = extension;
        ContentCategory = contentCategory;
        SiteCategory = siteCategory;
        Tags = tags;
        AlbumName = albumName;
    }


    public string Url { get; }
    public string? ThumbnailUrl { get; }
    public string FileName { get; }
    public string Extension { get; }
    public ContentCategory ContentCategory { get; }
    public string SiteCategory { get; }
    public List<string> Tags { get; }
    public string? AlbumName { get; }
    public string? FileContentAsBase64String { get; set; }
}