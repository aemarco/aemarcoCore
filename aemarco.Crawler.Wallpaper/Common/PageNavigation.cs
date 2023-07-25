namespace aemarco.Crawler.Wallpaper.Common;


internal record PageUri(Uri Uri)
{
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


    internal PageNode? FindNode(string xPath)
    {
        var node = Document.DocumentNode.SelectSingleNode(xPath);
        if (node is null)
            return null;
        return new PageNode(this, node);
    }
    internal IReadOnlyList<PageNode> FindNodes(string xPath)
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

    internal PageNode? GetChild(string xPath)
    {
        var subNode = Node.SelectSingleNode(xPath);
        if (subNode is null)
            return null;
        return new PageNode(this, subNode);
    }


    internal PageUri? GetHref() => GetRef("href");
    internal PageUri? GetSrc() => GetRef("src");
    private PageUri? GetRef(string attr)
    {
        var href = Node.Attributes[attr]?.Value;
        if (href is null)
            return null;
        var uri = new Uri(Uri, href);
        return new PageUri(uri);
    }

}




