namespace aemarco.Crawler.Common;

public record PageDocument(Uri Uri, HtmlDocument Document)
    : PageUri(Uri)
{
    public PageDocument(PageUri pageUri, HtmlDocument document)
        : this(pageUri.Uri, document)
    { }


    public virtual PageNode? FindNode(string xPath) =>
        Document.DocumentNode.SelectSingleNode(xPath) is { } subNode
            ? new PageNode(this, subNode)
            : null;

    public virtual List<PageNode> FindNodes(string xPath)
    {
        var nodes = Document.DocumentNode.SelectNodes(xPath) ?? Enumerable.Empty<HtmlNode>();
        var result = nodes
            .ToArray()
            .Select(x => new PageNode(this, x))
            .ToList();
        return result;
    }
}