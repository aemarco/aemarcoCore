using aemarco.Crawler.Wallpaper.Mapping;
using aemarco.Crawler.Wallpaper.Model.Clients;

namespace aemarco.Crawler.WallpaperTests;

public class WallhavenMapperTests
{

    [TestCase(null, new string[0])]
    [TestCase("", new string[0])]
    [TestCase("a, b", new[] { "a", "b" })]
    [TestCase("a, ,b", new[] { "a", "b" })]
    public void ToTag_Alias_SplitIntoList(string? alias, string[] expected)
    {
        var source = new WallhavenTag(1, "name", alias, 2, "Musicians", "sfw");

        var result = source.ToTag();

        result.Aliases.ShouldBe(expected);
    }

    [Test]
    public void ToTag_Tag_TextAndCategoryTaken()
    {
        var source = new WallhavenTag(1, "Tsuki (Billlie)", null, 65, "Musicians", "sfw");

        var result = source.ToTag();

        result.Text.ShouldBe("Tsuki (Billlie)");
        result.Category.ShouldBe("Musicians");
    }

    [Test]
    public void Tag_WithoutAliases_EmptyNotNull()
    {
        var result = new Tag("redhead");

        result.Aliases.ShouldBeEmpty();
    }

}
