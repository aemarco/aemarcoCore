namespace aemarco.Crawler.Common;

public record PageNode(Uri Uri, HtmlDocument Document, HtmlNode Node)
    : PageDocument(Uri, Document)
{
    public PageNode(PageDocument pageDocument, HtmlNode node)
        : this(pageDocument.Uri, pageDocument.Document, node)
    { }

    public override PageNode? FindNode(string xPath) =>
        Node.SelectSingleNode(xPath) is { } subNode
            ? new PageNode(this, subNode)
            : null;

    public override IReadOnlyList<PageNode> FindNodes(string xPath)
    {
        var nodes = Node.SelectNodes(xPath) ?? Enumerable.Empty<HtmlNode>();
        var result = nodes
            .ToArray()
            .Select(x => new PageNode(this, x))
            .ToList();
        return result;
    }

    //References
    public PageUri? GetHref(Func<string, string?>? manipulation = null) =>
        GetAttributeRef("href", manipulation);
    public PageUri? GetSrc(Func<string, string?>? manipulation = null) =>
        GetAttributeRef("src", manipulation);
    public PageUri? GetAttributeRef(string attr, Func<string, string?>? manipulation = null) =>
        GetAttribute(attr) is not { } attrHref
            ? null
            : manipulation is null
                ? new PageUri(new Uri(Uri, attrHref))
                : manipulation(attrHref) is { } manipulatedAttrHref
                    ? new PageUri(new Uri(Uri, manipulatedAttrHref))
                    : null;


    public string? GetAttribute(string attr) =>
        Node.Attributes[attr]?.Value;

    //Info
    public string GetText() =>
        WebUtility.HtmlDecode(Node.InnerText).Trim();


}
