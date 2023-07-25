// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;


[WallpaperCrawler("Zoomgirls")]
internal class Zoomgirls : WallpaperCrawlerBasis
{
    private readonly Uri _uri = new("https://zoomgirls.net");

    public Zoomgirls(int startPage, int lastPage, bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {
            CreateCrawlOffer(
                "Latest Wallpapers",
                new Uri(_uri, "latest_wallpapers"),
                new ContentCategory(Category.Girls)),
            CreateCrawlOffer(
                "Random Wallpapers",
                new Uri(_uri, "random_wallpapers"),
                new ContentCategory(Category.Girls))
        };

        return result;
    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
        //return $"{catJob.CategoryUri.AbsoluteUri}/page/{catJob.CurrentPage}";
        return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/page/{catJob.CurrentPage}");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='thumb']/a";
    }

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;
        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        var imageUri = GetImageUrl(source.DetailsDoc);
        if (string.IsNullOrEmpty(imageUri))
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }

        source.ImageUri = new PageUri(new Uri(imageUri));
        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='wallpaper-thumb']/img", "src");
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
        source.ContentCategory = catJob.Category;
        source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@class='tagcloud']/span/a", x => WebUtility.HtmlDecode(x.InnerText).Trim());

        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }
    private string? GetImageUrl(HtmlDocument doc)
    {
        HtmlNode? targetNode = null;

        //select all resolution nodes
        var allNodes = doc.DocumentNode.SelectNodes("//div[@class='tagcloud']/span/a");
        if (allNodes is null)
        {
            return null;
        }
        //search for node with highest resolution
        var maxSum = 0;
        foreach (var node in allNodes)
        {
            //get both number values
            var txt = node.Attributes["title"]?.Value?.Split('x');
            if (txt is not { Length: 2 })
                continue;

            int sum;
            try
            {//do the math
                sum = int.Parse(txt[0].Trim()) * int.Parse(txt[1].Trim());
            }
            catch
            {
                continue;
            }
            if (sum <= maxSum)
                continue;

            //set to targetNode because sum is highest
            maxSum = sum;
            targetNode = node;
        }

        //z.B. "/view-jana-jordan--1920x1200.html"
        var name = targetNode?.Attributes["href"]?.Value;
        if (name is null)
            return null;


        //z.B. "jana-jordan--1920x1200.html"
        name = name[(name.IndexOf("view", StringComparison.Ordinal) + 5)..];
        //z.B. "jana-jordan--1920x1200"
        name = name[..name.IndexOf(".html", StringComparison.Ordinal)];
        //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"

        var url = new Uri(_uri, $"/wallpapers/{name}.jpg").AbsoluteUri;

        return url;

    }


}