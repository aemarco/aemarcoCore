// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Wallhaven")]
internal class Wallhaven : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://wallhaven.cc");
    public Wallhaven(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {

            CreateCrawlOffer(
                "Anime_SFW",
                new PageUri(new Uri(_uri, @"search?q=&categories=010&purity=100&sorting=date_added&order=desc&ai_art_filter=0")),
                new ContentCategory(Category.Girls_Fantasy, 1, 19)),

            CreateCrawlOffer(
                "Anime_Sketchy",
                new PageUri(new Uri(_uri, @"search?q=&categories=010&purity=010&sorting=date_added&order=desc&ai_art_filter=0")),
                new ContentCategory(Category.Girls_Fantasy)),

            CreateCrawlOffer(
                "People_SFW",
                new PageUri(new Uri(_uri, @"search?q=&categories=001&purity=100&sorting=date_added&order=desc&ai_art_filter=0")),
                new ContentCategory(Category.Girls, 1, 19)),

            CreateCrawlOffer(
                "People_Sketchy",
                new PageUri( new Uri(_uri, @"search?q=&categories=001&purity=010&sorting=date_added&order=desc&ai_art_filter=0")),
                new ContentCategory(Category.Girls))

        };
        return result;
    }
    //z.B. "https://wallhaven.cc/search?q=&categories=001&purity=100&sorting=date_added&order=desc&ai_art_filter=0&page=1"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithParam("page", catJob.CurrentPage.ToString());
    protected override string GetSearchStringGorEntryNodes() =>
        "//ul/li/figure";
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //navigate
        if (pageNode
                .FindNode("./a[@class='preview']")?
                .GetHref()?
                .Navigate() is not { } detailsPage)
        {
            AddWarning(pageNode, "Could not find DetailsDoc");
            return false;
        }
        if (detailsPage
                .FindNode("//img[@id='wallpaper']")?
                .GetSrc() is not { } imageUri)
        {
            AddWarning(detailsPage, "Could not get ImageUri");
            return false;
        }

        //details
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = pageNode
                .FindNode("./img[@alt='loading']")?
                .GetAttributeRef("data-src")
        };
        source.AddTagsFromInnerTexts(detailsPage.FindNodes("//ul[@id='tags']/li"));

        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }

}