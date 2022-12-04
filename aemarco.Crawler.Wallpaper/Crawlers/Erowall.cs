// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;


[WallpaperCrawler("Erowall")]
internal class Erowall : WallpaperCrawlerBasis
{
    private readonly Uri _uri = new("https://erowall.com");



    public Erowall(
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

        //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
        foreach (var node in doc.DocumentNode.SelectNodes("//ul[@class='m']/li[@class='m']/a"))
        {

            //z.B. "#brunette"
            var text = WebUtility.HtmlDecode(node.InnerText).Trim();
            if (string.IsNullOrEmpty(text) || !text.StartsWith("#"))
            {
                continue;
            }

            //z.B. "brunette"
            text = text.Substring(1);
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }
            //z.B. "Brunette"
            text = char.ToUpper(text[0]) + text.Substring(1);
            var cat = GetContentCategory(text);
            if (cat is null)
            {
                continue;
            }


            //z.B. "/search/brunette/"
            var href = node.Attributes["href"]?.Value;
            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            //z.B. "search/brunette/"
            href = href.Replace("search", "teg");


            //z.B. "https://erowall.com/teg/brunette/"
            var uri = new Uri(_uri, href);
            result.Add(CreateCrawlOffer(text, uri, cat));
        }

        return result;

    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://erowall.com/teg/brunette/page/1"       
        //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}";
        return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}page/{catJob.CurrentPage}");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='wpmini']/a";
    }
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
    {
        var href = node.Attributes["href"]?.Value;

        if (href is null)
            return false;


        //details
        var match = Regex.Match(href, @"/(\d+)/$");
        // z.B. "24741"
        var imageLink = match.Groups[1].Value;

        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
            return false;

        //details
        source.ImageUri = new Uri(_uri, $"/wallpapers/original/{imageLink}.jpg");
        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='view-left']/a/img", "src");
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
        source.ContentCategory = catJob.Category;
        source.Tags = source.GetTagsFromNode(node, "title");


        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

}