using aemarco.Crawler.Extensions;
using Riok.Mapperly.Abstractions;

namespace aemarco.Crawler.Wallpaper.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class WallhavenMapper
{

    [MapProperty(nameof(WallhavenTag.Name), nameof(Tag.Text))]
    [MapProperty(nameof(WallhavenTag.Alias), nameof(Tag.Aliases), Use = nameof(MapAliases))]
    public static partial Tag ToTag(this WallhavenTag source);

    //wallhaven sends the aliases as one comma separated text
    private static IReadOnlyList<string> MapAliases(string? alias) =>
        [.. alias.SplitList()];

}
