using aemarco.Crawler.Common;
using aemarco.Crawler.Model;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;

namespace aemarco.Crawler.WallpaperTests;

public class WallEntrySourceTests
{

    [Test]
    public void ToWallEntry_TagsDifferingInCase_FirstOneWins()
    {
        var sut = new WallEntrySource(new ContentCategory(Category.Girls), "site")
        {
            ImageUri = new PageUri(new Uri("https://example.com/a.jpg")),
            Tags =
            [
                new Tag("Redhead", "People", ["Ginger"]),
                new Tag("redhead"),
                new Tag("blonde")
            ]
        };

        var result = sut.ToWallEntry();

        result.ShouldNotBeNull().Tags.ShouldBe(
        [
            new Tag("Redhead", "People", ["Ginger"]),
            new Tag("blonde")
        ]);
    }

}
