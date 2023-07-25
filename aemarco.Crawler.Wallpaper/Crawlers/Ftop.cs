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

            //z.B. "celebrities/index.html
            var href = node.Attributes["href"]?.Value;
            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            //z.B. "https://ftopx.com/celebrities"
            var uri = new Uri(_uri, href[..href.LastIndexOf('/')]);
            result.Add(CreateCrawlOffer(text, uri, cat));
        }

        return result;

    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "http://ftopx.com/celebrities/
        var res = catJob.CategoryUri.AbsoluteUri;
        //z.B. "http://ftopx.com/celebrities/
        res = res.EndsWith('/') ? res : $"{res}/";
        //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc
        var result = new Uri($"{res}page/{catJob.CurrentPage}/?sort=p.approvedAt&direction=desc");
        return result;
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='thumbnail']/a";
    }

    protected override ContentCategory DefaultCategory => new(Category.Girls);

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        pageNode = new PageNode(pageNode, pageNode.Node.ParentNode.ParentNode);
        var source = new WallEntrySource(_uri, pageNode, catJob.SiteCategoryName);


        //docs
        source.DetailsPageDocument = pageNode
            .GetChild("./div[@class='thumbnail']/a")?
            .GetHref()?
            .Navigate();
        if (source.DetailsPageDocument is null)
        {
            AddWarning($"Could not find DetailsDoc from {pageNode.Node.InnerHtml}");
            return false;
        }

        source.DownloadPageDocument = source.DetailsPageDocument
            .FindNode("//div[@class='res-origin']/a")?
            .GetHref()?
            .Navigate();
        if (source.DownloadPageDocument is null || source.DownloadDoc is null)
        {
            AddWarning($"Could not find DownloadDoc from {source.DetailsPageDocument.Document.DocumentNode.InnerHtml}");
            return false;
        }


        //details
        source.ImageUri = source.DownloadPageDocument
            .FindNode("//a[@type='button']")?
            .GetHref()?
            .Uri;
        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DownloadDoc.DocumentNode.InnerHtml}");
            return false;
        }


        source.ThumbnailUri = source.DetailsPageDocument
            .FindNode("//img[@class='img-responsive img-rounded']")?
            .GetSrc()?
            .Uri;


        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
        source.ContentCategory = catJob.Category;
        source.Tags = source.GetTagsFromNodes(source.DetailsPageDocument.Document, "//div[@class='well well-sm']/a", x => WebUtility.HtmlDecode(x.InnerText).Trim());


        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

}