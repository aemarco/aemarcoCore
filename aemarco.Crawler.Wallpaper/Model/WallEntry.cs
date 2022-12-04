// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace aemarco.Crawler.Wallpaper.Model;

public class WallEntry
{

    internal WallEntry(
        string? url,
        string? thumbnailUrl,
        string? fileName,
        string? extension,
        ContentCategory? contentCategory,
        string? siteCategory,
        List<string>? tags,
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


    public string? Url { get; }
    public string? ThumbnailUrl { get; }
    public string? FileName { get; }
    public string? Extension { get; }
    public ContentCategory? ContentCategory { get; }
    public string? SiteCategory { get; }
    public List<string>? Tags { get; }
    public string? AlbumName { get; }
    public string? FileContentAsBase64String { get; set; }


    private static List<string>? _crawlerExtensions;
    [JsonIgnore]
    internal bool IsValid
    {
        get
        {
            //Once initialized with valid extensions
            _crawlerExtensions ??= new List<string>
            {
                ".bmp",
                ".jpg",
                ".jpeg",
                ".png",
                ".gif"
            };

            if (string.IsNullOrEmpty(Url))
                return false;
            if (string.IsNullOrEmpty(ThumbnailUrl))
                return false;
            if (string.IsNullOrEmpty(FileName))
                return false;
            if (string.IsNullOrEmpty(Extension) || !_crawlerExtensions.Contains(Extension))
                return false;
            if (ContentCategory is null)
                return false;
            if (string.IsNullOrEmpty(SiteCategory))
                return false;
            if (Tags == null)
                return false;

            return true;
        }
    }
}