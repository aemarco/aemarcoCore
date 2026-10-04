// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[Crawler("Boombo")]
internal class Boombo : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://boombo.biz");
    public Boombo(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var mainPage = new PageUri(new Uri(_uri, "en/")).Navigate();
        //the categories of the mega menu, a link each, e.g. "Anal Sex" -> "https://boombo.biz/en/anal/"
        var catNodes = mainPage.FindNodes("//div[@class='submenu mega-menu']//ul/li/a");

        var result = new List<CrawlOffer>();
        foreach (var catNode in catNodes)
        {

            //z.B. "Nude photo"
            if (catNode.GetText() is not { Length: > 0 } cat)
                continue;

            //z.B. "/en/nude/
            if (catNode.GetHref() is not { } catPage)
                continue;

            result.Add(CreateCrawlOffer(
                cat,
                catPage,
                new ContentCategory(Category.Girls)));
        }

        return result;
    }
    //z.B. "https://boombo.biz/en/nude/page/1/"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringGorEntryNodes() =>
        "//div[@class='fonwall-item']/a";
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {

        //navigate
        if (pageNode
                .GetHref()?
                .Navigate() is not { } albumPage)
        {
            AddWarning(pageNode, "Could not find AlbumDoc");
            return false;
        }
        if (pageNode
                .FindNode(".//span[@class='fonwall-title']")?
                .GetText() is not { Length: > 0 } albumName)
        {
            AddWarning(pageNode, "Could not find AlbumName");
            return false;
        }


        var album = new AlbumEntry(albumName);
        var entryNodes = albumPage.FindNodes("//a[@class='highslide']");
        foreach (var entryNode in entryNodes)
        {
            //details
            if (entryNode
                    .GetHref() is not { } imageUri)
            {
                AddWarning(entryNode, "Could not get ImageUri");
                continue;
            }
            var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
            {
                AlbumName = albumName,
                ImageUri = imageUri
            };
            source.AddTagsFromText(entryNode.FindNode("./img")?.GetAttribute("alt"));

            //entry
            if (source.ToWallEntry() is not { } entry)
                continue;

            album.Entries.Add(entry);
        }

        var entryNodes2 = albumPage.FindNodes("//div[@class='text']/div/img");
        foreach (var entryNode in entryNodes2)
        {
            //details
            if (entryNode
                    .GetAttributeRef("data-src") is not { } imageUri)
            {
                AddWarning(entryNode, "Could not get ImageUri");
                return false;
            }
            var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
            {
                AlbumName = albumName,
                ImageUri = imageUri
            };
            source.AddTagsFromText(entryNode.GetAttribute("alt"));

            //entry
            if (source.ToWallEntry() is not { } entry)
                continue;

            album.Entries.Add(entry);
        }

        if (album.Entries.Count == 0)
            return false;

        album.Entries.Sort(new WallEntryNumberComparer());
        AddAlbum(album, catJob);
        return true;

    }

}
public partial class WallEntryNumberComparer : IComparer<WallEntry>
{
    public int Compare(WallEntry? x, WallEntry? y) =>
        GetNumber(x).CompareTo(GetNumber(y));


    [GeneratedRegex(@"(\d+)(?!.*\d)")]
    private static partial Regex EndNumberRegex();

    private static int GetNumber(WallEntry? x)
    {
        if (x is null)
            return 0;

        if (EndNumberRegex().Match(x.Url) is not { Success: true } match)
            return 0;

        return int.Parse(match.Groups[1].Value);
    }
}
