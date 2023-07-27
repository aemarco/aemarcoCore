// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Zoomgirls")]
internal class Zoomgirls : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://zoomgirls.net");
    public Zoomgirls(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {

            //specific
            CreateCrawlOffer(
                "Girls and Bikes",
                new PageUri(new Uri(_uri, "girls-and-bikes-desktop-wallpapers/")),
                new ContentCategory(Category.Girls_Bikes)),
            CreateCrawlOffer(
                "Girls and Cars",
                new PageUri(new Uri(_uri, "girls-and-cars-desktop-wallpapers/")),
                new ContentCategory(Category.Girls_Cars)),
            CreateCrawlOffer(
                "Hentai",
                new PageUri(new Uri(_uri, "hentai-desktop-wallpapers/")),
                new ContentCategory(Category.Girls_Fantasy)),
            
            //less specific
            CreateCrawlOffer(
                "Dildo Girls",
                new PageUri(new Uri(_uri, "babes-and-dildos-desktop-wallpapers/")),
                new ContentCategory(Category.Girls, 70, 79)),
            CreateCrawlOffer(
                "Nude Girls",
                new PageUri(new Uri(_uri, "sexy-nude-girls-desktop-wallpapers/")),
                new ContentCategory(Category.Girls)),
            CreateCrawlOffer(
                "Sexy Girls",
                new PageUri(new Uri(_uri, "sexy-girls-desktop-wallpapers/")),
                new ContentCategory(Category.Girls, 30, 39)),
            
            //category list on site is not comprehensive
            CreateCrawlOffer(
                "Latest Wallpapers",
                new PageUri(new Uri(_uri, "latest_wallpapers/")),
                new ContentCategory(Category.Girls)),
        };

        return result;
    }
    //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringGorEntryNodes() =>
        "//div[@class='thumb']/a";
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //navigate
        if (pageNode
                .GetHref()?
                .Navigate() is not { } detailsPage)
        {
            AddWarning(pageNode, "Could not find DetailsDoc");
            return false;
        }
        if (GetImageUrl(detailsPage) is not { } imageUri)
        {
            AddWarning(detailsPage, "Could not get ImageUri");
            return false;
        }

        //details
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = detailsPage
                .FindNode("//a[@class='wallpaper-thumb']/img")?
                .GetSrc()
        };
        source.AddTagsFromInnerTexts(detailsPage.FindNodes("//ul[@class='holder tags']/a"));
        if (catJob.SiteCategoryName == "Latest Wallpapers")
        {// on general category, we try to find a more specific category through offers
            var categories = detailsPage
                .FindNodes("//itemscope/a")
                .Select(x => x.GetText())
                .ToList();
            if (categories.Count > 0 &&
                GetCrawlsOffers().FirstOrDefault(o => categories.Contains(o.SiteCategoryName)) is { } match)
            {
                source.OverrideCategory(match.Category);
            }
        }

        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }
    private PageUri? GetImageUrl(PageDocument doc)
    {



        //select all resolution nodes
        var resNodes = doc.FindNodes("//div[@class='tagcloud']/span/a");

        //search for node with highest resolution
        var maxPix = 0;
        PageNode? best = null;
        foreach (var resNode in resNodes)
        {
            if (resNode
                    .GetAttribute("title")?
                    .Split('x', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                is not { Length: 2 } pix)
            {
                continue;
            }

            if (!int.TryParse(pix[0], out var width) ||
                !int.TryParse(pix[1], out var height))
            {
                continue;
            }


            var pixels = width * height;
            if (pixels <= maxPix)
            {
                continue;
            }

            //set to targetNode because highest resolution
            maxPix = pixels;
            best = resNode;
        }


        //z.B. "/view-jana-jordan--1920x1200.html"
        if (best?.GetAttribute("href") is not { } href ||
            !href.StartsWith("/view-") ||
            !href.EndsWith(".html"))
            return null;

        //z.B. "jana-jordan--1920x1200.html"
        var name = href[(href.IndexOf("view", StringComparison.OrdinalIgnoreCase) + 5)..];

        //z.B. "jana-jordan--1920x1200"
        name = name[..name.IndexOf(".html", StringComparison.OrdinalIgnoreCase)];

        //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"
        var result = new PageUri(new Uri(_uri, $"/wallpapers/{name}.jpg"));
        return result;
    }

}