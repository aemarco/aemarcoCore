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
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {

            CreateCrawlOffer(
                "Anime_SFW",
                new Uri(_uri, @"search?q=&categories=010&purity=100&sorting=date_added&order=desc"),
                new ContentCategory(Category.Girls_Fantasy, 1, 19)),

            CreateCrawlOffer(
                "Anime_Sketchy",
                new Uri(_uri, @"search?q=&categories=010&purity=010&sorting=date_added&order=desc"),
                new ContentCategory(Category.Girls_Fantasy)),

            CreateCrawlOffer(
                "People_SFW",
                new Uri(_uri, @"search?q=&categories=001&purity=100&sorting=date_added&order=desc"),
                new ContentCategory(Category.Girls, 1, 19)),

            CreateCrawlOffer(
                "People_Sketchy",
                new Uri(_uri, @"search?q=&categories=001&purity=010&sorting=date_added&order=desc"),
                new ContentCategory(Category.Girls))


        };
        return result;
    }

    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {

        //z.B. "https://alpha.wallhaven.cc/search?q=&categories=001&purity=010&sorting=date_added&order=desc&page=1"
        return new Uri($"{catJob.CategoryUri.AbsoluteUri}&page={catJob.CurrentPage}");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//ul/li/figure";

        //return "//div[@class='thumb']/a";
    }

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;
        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);
        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode("./a[@class='preview']");
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@id='wallpaper']", "src");
        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }
        source.ThumbnailUri = new Uri(_uri, source.GetSubNodeAttribute(node, "data-src", "./img[@alt='loading']"));
        source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@id='tags']/li", x => WebUtility.HtmlDecode(x.InnerText).Trim());
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
        source.ContentCategory = catJob.Category;


        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }




}