// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Ftop")]
internal class Ftop : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://ftopx.com");



    public Ftop(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();

        //main page
        var doc = HtmlHelper.GetHtmlDocument(_uri);

        foreach (var node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
        {

            //z.B. "Celebrities"
            var text = WebUtility.HtmlDecode(node.InnerText).Trim();
            if (string.IsNullOrEmpty(text) || text == "Sandbox")
            {
                continue;
            }
            var cat = GetContentCategory(text);
            if (cat is null)
            {
                continue;
            }

            //z.B. "/celebrities/
            var href = node.Attributes["href"]?.Value;
            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            //z.B. "https://ftopx.com/celebrities"
            var uri = new Uri(_uri, href);
            result.Add(CreateCrawlOffer(text, uri, cat));
        }

        return result;

    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
        //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}/?sort=p.approvedAt&direction=desc";
        return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}page/{catJob.CurrentPage}/?sort=p.approvedAt&direction=desc");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='thumbnail']/a";
    }

    protected override ContentCategory DefaultCategory => new(Category.Girls);

    protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
    {
        node = node.ParentNode.ParentNode;

        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode("./div[@class='thumbnail']/a");
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }
        source.DownloadDoc = source.GetChildDocumentFromDocument(source.DetailsDoc, "//div[@class='res-origin']/a");
        if (source.DownloadDoc is null)
        {
            AddWarning($"Could not read DownloadDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//a[@type='button']", "href");
        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DownloadDoc.DocumentNode.InnerHtml}");
            return false;
        }

        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='img-responsive img-rounded']", "src");
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
        source.ContentCategory = catJob.Category;
        source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='well well-sm']/a", x => WebUtility.HtmlDecode(x.InnerText).Trim());



        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

}