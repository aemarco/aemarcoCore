namespace aemarco.Crawler.Model;

/// <summary>
/// a tag as a site delivers it; sites without categories or aliases just carry the text
/// </summary>
/// <param name="Text">the tag itself</param>
/// <param name="Category">the site's own category of the tag, e.g. "Musicians"</param>
/// <param name="Aliases">other names of the tag, empty when there are none</param>
public sealed record Tag(
    string Text,
    string? Category = null,
    IReadOnlyList<string>? Aliases = null)
{
    public IReadOnlyList<string> Aliases { get; init; } = Aliases ?? [];

    //the record would compare the aliases by reference, so equality is written out: same text, category and aliases in the same order
    public bool Equals(Tag? other) =>
        other is not null &&
        Text == other.Text &&
        Category == other.Category &&
        Aliases.SequenceEqual(other.Aliases);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Text);
        hash.Add(Category);
        foreach (var alias in Aliases)
            hash.Add(alias);
        return hash.ToHashCode();
    }

    public override string ToString() => Text;
}
