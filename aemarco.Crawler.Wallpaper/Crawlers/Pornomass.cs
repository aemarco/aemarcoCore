
// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;


/// <summary>
/// The site delivers walls in the HC+ range
/// The site does not add new content since a long time
/// </summary>
[WallpaperCrawler("Pornomass")]
internal class Pornomass : WallpaperCrawlerBasis
{
    private readonly Uri _uri = new("http://pornomass.com");

    public Pornomass(
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
                "Pornomass",
                _uri,
                new ContentCategory(Category.Girls, 90, 99))
        };
        return result;
    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "http://pornomass.com/page/1"
        //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}";
        return new Uri(catJob.CategoryUri, $"/page/{catJob.CurrentPage}");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='fit-box']/a[@class='fit-wrapper']";
    }
    protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
    {
        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //doc
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }
        //details
        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']/img", "src");
        //details
        source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']", "href");
        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
        source.ContentCategory = catJob.Category;
        source.Tags = new List<string>();



        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

}