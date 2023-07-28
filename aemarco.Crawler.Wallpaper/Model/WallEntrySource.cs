// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarco.Crawler.Wallpaper.Model;

internal class WallEntrySource
{

    private static readonly string[] AllowedExtensions =
    {
        ".bmp",
        ".jpg",
        ".jpeg",
        ".png",
        ".gif"
    };
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


    public List<string> Tags { get; set; } = new();
    public void AddTagsFromText(string? text, params char[] separators)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        separators = separators.Length == 0
            ? new[] { ',' }
            : separators;
        Tags.AddRange(WebUtility.HtmlDecode(text)
            .Split(separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
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
        var main = url[(url.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
        main = WebUtility.HtmlDecode(main);
        var result = Path.GetFileNameWithoutExtension(main);
        return result;
    }
    private static string ExtensionFromUrl(PageUri uri)
    {
        var url = uri.Uri.AbsoluteUri;
        var main = url[(url.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
        main = WebUtility.HtmlDecode(main);
        var result = Path.GetExtension(main);
        return result;
    }






    //old stuff



    #region navigation

    internal static string? GetSubNodeAttrib(HtmlNode node, string attribute, string? nodeToTargetNode = null)
    {
        var subNode = nodeToTargetNode is null
            ? node
            : node.SelectSingleNode(nodeToTargetNode);

        var value = subNode?.Attributes[attribute]?.Value;
        return value;
    }


    #endregion

    #region tags

    internal static List<string> GetTagsFromNodes(
        HtmlDocument doc,
        string docToTargetNodes,
        Func<HtmlNode, string?> selector)
    {
        var nodes = doc.DocumentNode.SelectNodes(docToTargetNodes);
        var tags = nodes?.Select(selector).Where(x => x is not null).ToList();
        return tags is null
            ? new List<string>()
            : GetTagsFromTagString(string.Join(",", tags));
    }

    private static List<string> GetTagsFromTagString(string tagString)
    {
        //z.B. "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"
        var result = new List<string>();

        result.AddRange(WebUtility.HtmlDecode(tagString)
            .Split(',')
            .Select(tag => tag.Trim())
            .Where(entry => entry.Length > 0));

        return result;
    }

    #endregion

    public WallEntry? WallEntry => ToWallEntry();
}