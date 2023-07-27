namespace aemarco.Crawler.Wallpaper.Common;


internal record PageUri(Uri Uri)
{

    /// <summary>
    ///
    /// - pageXy --> relative to current
    /// - /pageXy --> relative to root
    /// - ../ --> relative to parent
    /// </summary>
    /// <param name="relativeUri"></param>
    /// <returns></returns>
    internal PageUri WithHref(string relativeUri)
    {
        var uri = new Uri(Uri, relativeUri);
        return new PageUri(uri);
    }

    internal PageDocument Navigate()
    {
        var document = HtmlHelper.GetHtmlDocument(Uri);
        return new PageDocument(this, document);
    }


    public static implicit operator Uri(PageUri x) => x.Uri;
    public static implicit operator PageUri?(Uri? x) => x is null ? null : new PageUri(x);
}


internal record PageDocument(Uri Uri, HtmlDocument Document)
    : PageUri(Uri)
{
    internal PageDocument(PageUri pageUri, HtmlDocument document)
        : this(pageUri.Uri, document)
    { }


    internal virtual PageNode? FindNode(string xPath) =>
        Document.DocumentNode.SelectSingleNode(xPath) is { } subNode
            ? new PageNode(this, subNode)
            : null;

    internal virtual IReadOnlyList<PageNode> FindNodes(string xPath)
    {
        var nodes = Document.DocumentNode.SelectNodes(xPath) ?? Enumerable.Empty<HtmlNode>();
        var result = nodes
            .ToArray()
            .Select(x => new PageNode(this, x))
            .ToList();
        return result;
    }
}



internal record PageNode(Uri Uri, HtmlDocument Document, HtmlNode Node)
    : PageDocument(Uri, Document)
{
    internal PageNode(PageDocument pageDocument, HtmlNode node)
        : this(pageDocument.Uri, pageDocument.Document, node)
    { }

    internal override PageNode? FindNode(string xPath) =>
        Node.SelectSingleNode(xPath) is { } subNode
            ? new PageNode(this, subNode)
            : null;

    internal override IReadOnlyList<PageNode> FindNodes(string xPath)
    {
        var nodes = Node.SelectNodes(xPath) ?? Enumerable.Empty<HtmlNode>();
        var result = nodes
            .ToArray()
            .Select(x => new PageNode(this, x))
            .ToList();
        return result;
    }

    //References
    internal PageUri? GetHref(Func<string, string?>? manipulation = null) =>
        GetAttribute("href") is not { } href
            ? null
            : manipulation is null
                ? new PageUri(new Uri(Uri, href))
                : manipulation(href) is { } manipulatedHref
                    ? new PageUri(new Uri(Uri, manipulatedHref))
                    : null;

    internal PageUri? GetSrc() =>
        GetAttribute("src") is { } href
            ? new PageUri(new Uri(Uri, href))
            : null;
    internal string? GetAttribute(string attr) =>
        Node.Attributes[attr]?.Value;


    //Info
    internal string GetText() =>
        WebUtility.HtmlDecode(Node.InnerText).Trim();


}




