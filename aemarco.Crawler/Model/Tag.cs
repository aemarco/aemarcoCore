namespace aemarco.Crawler.Model;

/// <summary>
/// a tag as a site delivers it; sites without categories or aliases just carry the text
/// </summary>
/// <param name="Text">the tag itself</param>
/// <param name="Category">the site's own category of the tag, e.g. "Musicians"</param>
/// <param name="Aliases">other names of the tag</param>
public record Tag(
    string Text,
    string? Category = null,
    IReadOnlyList<string>? Aliases = null)
{
    public override string ToString() => Text;
}
