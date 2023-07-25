// ReSharper disable MemberCanBePrivate.Global
namespace aemarco.Crawler.Wallpaper.Model;

internal class WallEntrySource
{

    #region ctor

    private readonly Uri _baseUri;


    internal WallEntrySource(Uri baseUri, HtmlNode rootNode, string siteCategory)
    {
        _baseUri = baseUri;
        RootNode = rootNode;
        SiteCategory = siteCategory;
    }

    internal WallEntrySource(Uri baseUri, PageNode pageNode, string siteCategory)
        : this(baseUri, pageNode.Node, siteCategory)
    { }


    internal HtmlNode RootNode { get; }


    #endregion

    #region Output

    internal Uri? ImageUri { get; set; }
    internal Uri? ThumbnailUri { get; set; }
    public string? Filename { get; set; }
    public string? Extension { get; set; }
    public ContentCategory? ContentCategory { get; set; }
    public string SiteCategory { get; set; }
    public List<string>? Tags { get; set; }
    public string? AlbumName { get; set; }


    internal WallEntry? WallEntry
    {
        get
        {
            var entry = new WallEntry(
                ImageUri?.AbsoluteUri,
                ThumbnailUri?.AbsoluteUri,
                Filename,
                Extension,
                ContentCategory,
                SiteCategory,
                Tags,
                AlbumName);

            return entry.IsValid
                ? entry
                : null;
        }
    }


    #endregion

    #region Documents

    private PageDocument? _detailsPageDocument;
    internal PageDocument? DetailsPageDocument
    {
        get => _detailsPageDocument;
        set
        {
            _detailsPageDocument = value;
            DetailsDoc = _detailsPageDocument?.Document;
        }
    }
    internal HtmlDocument? DetailsDoc { get; set; }


    private PageDocument? _downloadPageDocument;
    internal PageDocument? DownloadPageDocument
    {
        get => _downloadPageDocument;
        set
        {
            _downloadPageDocument = value;
            DownloadDoc = _downloadPageDocument?.Document;
        }
    }
    internal HtmlDocument? DownloadDoc { get; set; }



    internal HtmlDocument? GetChildDocumentFromRootNode(string? nodeToSubNode = null, int? minDelay = null, int? maxDelay = null)
    {
        return GetChildDocumentFromNode(RootNode, nodeToSubNode, minDelay, maxDelay);
    }
    internal HtmlDocument? GetChildDocumentFromNode(HtmlNode node, string? nodeToSubNode = null, int? minDelay = null, int? maxDelay = null)
    {
        var subNode = nodeToSubNode is null
            ? node :
            node.SelectSingleNode(nodeToSubNode);

        if (subNode is null)
            return null;

        var href = subNode.Attributes["href"]?.Value;

        //var curr = new Uri("https://ftopx.com/celebrities/page/1/");
        //var test = new Uri(curr, href);

        var uri = new Uri(_baseUri, href);
        return HtmlHelper.GetHtmlDocument(uri, minDelay, maxDelay);
    }
    internal HtmlDocument? GetChildDocumentFromDocument(HtmlDocument doc, string docToHrefNode, int? minDelay = null, int? maxDelay = null)
    {
        var node = doc.DocumentNode.SelectSingleNode(docToHrefNode);
        return node != null
            ? GetChildDocumentFromNode(node, null, minDelay, maxDelay)
            : null;
    }

    #endregion

    #region navigation

    internal Uri? GetUriFromDocument(HtmlDocument doc, string docToTargetNode, string attribute)
    {
        var node = doc.DocumentNode.SelectSingleNode(docToTargetNode);
        var href = node?.Attributes[attribute]?.Value;


        return href is null
            ? null :
            new Uri(_baseUri, href);
    }


    internal string? GetSubNodeAttribute(HtmlNode node, string attribute, string? nodeToTargetNode = null)
        => GetSubNodeAttrib(node, attribute, nodeToTargetNode);

    internal static string? GetSubNodeAttrib(HtmlNode node, string attribute, string? nodeToTargetNode = null)
    {
        var subNode = nodeToTargetNode is null
            ? node
            : node.SelectSingleNode(nodeToTargetNode);

        var value = subNode?.Attributes[attribute]?.Value;
        return value;
    }

    internal static string? GetSubNodeInnerText(HtmlNode node, string? nodeToTargetNode = null)
    {
        var subNode = nodeToTargetNode is null
            ? node
            : node.SelectSingleNode(nodeToTargetNode);

        var value = subNode?.InnerText.Trim();
        return value;
    }

    #endregion

    #region filedetails

    internal (string filename, string extension) GetFileDetails(Uri imageUri, string? prefix = null)
    {
        var pref = prefix is null
            ? string.Empty
            : $"{prefix}_";

        var url = imageUri.AbsoluteUri;
        var main = url[(url.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
        main = WebUtility.HtmlDecode(main);
        var name = $"{pref}{main}";

        return (Path.GetFileNameWithoutExtension(name), Path.GetExtension(name));
    }


    #endregion

    #region tags

    internal List<string> GetTagsFromNode(HtmlNode node, string attribute, string? nodeToTargetNode = null)
    {
        var subNode = nodeToTargetNode is null
            ? node
            : node.SelectSingleNode(nodeToTargetNode);
        var tagString = subNode?.Attributes[attribute]?.Value;

        return tagString is null
            ? new List<string>()
            : GetTagsFromTagString(tagString);
    }
    internal List<string> GetTagsFromNodes(
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

    private List<string> GetTagsFromTagString(string tagString)
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
}