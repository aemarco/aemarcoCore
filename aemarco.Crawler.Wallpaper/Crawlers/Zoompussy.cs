// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

//TODO modernize
[WallpaperCrawler("Zoompussy")]
internal class Zoompussy : WallpaperCrawlerBasis
{
    private readonly Uri _uri = new("http://zoompussy.com/");


    public Zoompussy(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();
        var cats = new List<string>
        {
            "asian",
            "ass",
            "bikini",
            "blonde",
            "boobs",
            "ebony",
            "tits",
            "brunette",
            "legs",
            "lingerie",
            "models",
            "naked",
            "nude",
            "pussy",
            "redhead",
            "stockings"
        };

        foreach (var cat in cats)
        {
            var cc = GetContentCategory(cat);
            if (cc is null)
                continue;

            result.Add(CreateCrawlOffer(
                cat,
                new Uri(_uri, $"/search/{cat}/")!,
                cc));
        }
        return result;
    }
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "http://zoompussy.com/search/asian/page/1/"
        return new Uri($"{catJob.CategoryUri.Uri.AbsoluteUri}page/{catJob.CurrentPage}")!;
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//li/div[@class='thumb']/a";
    }
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/blockquote/a", "href");

        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }
        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/blockquote/a/img", "src");
        source.SetFilenamePrefix(catJob.SiteCategoryName);
        source.Tags = WallEntrySource.GetTagsFromNodes(
            source.DetailsDoc,
            "//div[@class='post_z']/a",
            x =>
            {
                if (x.Attributes["href"] != null &&
                    x.Attributes["href"].Value.StartsWith(_uri.AbsoluteUri))
                {
                    return WebUtility.HtmlDecode(x.InnerText).Trim();
                }
                return null;
            });


        var wallEntry = source.WallEntry;
        if (wallEntry is null)
            return false;


        //damn.... no fun without referer --> special treatment needed :(
        //source.DownloadWithReferer(wallEntry, $"{_uri.AbsoluteUri}{wallEntry.FileName.ToLower()}");
        //if (!wallEntry.IsValid || string.IsNullOrWhiteSpace(wallEntry.FileContentAsBase64String)) 
        //    return false;


        AddEntry(wallEntry, catJob);
        return true;
    }
}