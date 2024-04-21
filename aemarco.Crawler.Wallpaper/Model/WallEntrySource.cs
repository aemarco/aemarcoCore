// ReSharper disable UnusedAutoPropertyAccessor.Global

using aemarco.Crawler.Extensions;

namespace aemarco.Crawler.Wallpaper.Model;

internal class WallEntrySource
{

    private static readonly string[] AllowedExtensions =
    [
        ".bmp",
        ".jpg",
        ".jpeg",
        ".png",
        ".gif"
    ];
    public WallEntrySource(ContentCategory contentCategory, string siteCategory)
    {
        _contentCategory = contentCategory;
        _siteCategory = siteCategory;
    }


    public PageUri? ImageUri { get; set; }
    public PageUri? ThumbnailUri { get; set; }
    public string? AlbumName { get; set; }


    private string? _filenamePrefix;
    public void SetFilenamePrefix(string prefix)
    {
        _filenamePrefix = WebUtility.HtmlDecode(prefix);
    }
    public string? Filename { get; set; }
    public string? Extension { get; set; }

    private ContentCategory _contentCategory;
    public void OverrideCategory(ContentCategory category)
    {
        _contentCategory = category;
    }
    private readonly string _siteCategory;


    public List<string> Tags { get; set; } = [];
    public void AddTagsFromText(string? text, params char[] separators)
    {
        Tags.AddRange(WebUtility.HtmlDecode(text).SplitList(separators));
    }
    public void AddTagsFromInnerTexts(IEnumerable<PageNode> nodes)
    {
        var tags = nodes
            .Select(x => WebUtility.HtmlDecode(x.Node.InnerText).Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x));
        Tags.AddRange(tags);
    }



    public WallEntry? ToWallEntry()
    {
        if (ImageUri is null)
            return null;

        var fileName = Filename ?? FilenameFromUrl(ImageUri);

        var entry = new WallEntry(
            ImageUri.Uri.AbsoluteUri,
            ThumbnailUri?.Uri.AbsoluteUri,
            string.IsNullOrWhiteSpace(_filenamePrefix)
                ? fileName
                : $"{_filenamePrefix}_{fileName}",
            Extension ?? ExtensionFromUrl(ImageUri),
            _contentCategory,
            _siteCategory,
            Tags.Distinct().ToList(),
            AlbumName);

        //extension must be allowed
        if (!AllowedExtensions.Contains(entry.Extension.ToLower()))
            return null;


        return entry;
    }
    private static string FilenameFromUrl(PageUri uri)
    {
        var url = uri.Uri.AbsoluteUri;
        var main = url[(url.LastIndexOf('/') + 1)..];
        main = WebUtility.HtmlDecode(main);
        var result = Path.GetFileNameWithoutExtension(main);
        return result;
    }
    private static string ExtensionFromUrl(PageUri uri)
    {
        var url = uri.Uri.AbsoluteUri;
        var main = url[(url.LastIndexOf('/') + 1)..];
        main = WebUtility.HtmlDecode(main);
        var result = Path.GetExtension(main);
        return result;
    }

}