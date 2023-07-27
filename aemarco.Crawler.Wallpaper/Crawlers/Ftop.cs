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

        var mainPage = new PageUri(_uri).Navigate();
        var catNodes = mainPage.FindNodes("//ul[@role='menu']/li/a");
        foreach (var catNode in catNodes)
        {
            //z.B. "Celebrities"
            var catName = catNode.GetText();
            if (string.IsNullOrEmpty(catName) || catName == "Sandbox")
                continue;

            var cat = GetContentCategory(catName);
            if (cat is null)
                continue;

            //z.B. ""https://ftopx.com/celebrities/index.html => "https://ftopx.com/celebrities"
            if (catNode.GetHref(x => x[..(x.LastIndexOf('/') + 1)]) is not { } uri)
                continue;

            result.Add(CreateCrawlOffer(catName, uri, cat));
        }

        return result;

    }
    //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}/?sort=p.approvedAt&direction=desc");
    protected override string GetSearchStringGorEntryNodes() =>
        "//div[@class='thumbnail']/a";
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        pageNode = new PageNode(pageNode, pageNode.Node.ParentNode.ParentNode);

        //navigate
        if (pageNode
                .FindNode("./div[@class='thumbnail']/a")?
                .GetHref()?
                .Navigate() is not { } detailsPage)
        {
            AddWarning(pageNode, "Could not find DetailsDoc");
            return false;
        }

        if (detailsPage
                .FindNode("//div[@class='res-origin']/a")?
                .GetHref()?
                .Navigate() is not { } downloadPage)
        {
            AddWarning(detailsPage, "Could not find DownloadDoc");
            return false;
        }
        if (downloadPage
                     .FindNode("//a[@type='button']")?
                     .GetHref() is not { } imageUri)
        {
            AddWarning(detailsPage, "Could not get valid DownloadDoc");
            return false;
        }


        //details
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = detailsPage
                .FindNode("//img[@class='img-responsive img-rounded']")?
                .GetSrc()
        };
        source.SetFilenamePrefix(catJob.SiteCategoryName);
        source.AddTagsFromInnerTexts(detailsPage.FindNodes("//div[@class='well well-sm']/a"));


        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }

}