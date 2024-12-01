//// ReSharper disable UnusedType.Global
//namespace aemarco.Crawler.Wallpaper.Crawlers.Obsolete;


///// <summary>
///// The site delivers walls in the HC+ range
///// The site does not add new content since a long time
///// </summary>
//[Crawler("Pornomass")]
//internal class Pornomass : WallpaperCrawlerBasis
//{

//    private readonly Uri _uri = new("http://pornomass.com");
//    public Pornomass(
//        int startPage,
//        int lastPage,
//        bool onlyNews)
//        : base(startPage, lastPage, onlyNews)
//    { }

//    protected override List<CrawlOffer> GetCrawlsOffers()
//    {
//        var result = new List<CrawlOffer>
//        {
//            CreateCrawlOffer(
//                "Pornomass",
//                new PageUri(_uri),
//                new ContentCategory(Category.Girls_Hardcore, 95, 100))
//            //ca 85% are >= 95
//        };
//        return result;
//    }
//    //z.B. "http://pornomass.com/page/1"
//    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
//        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
//    protected override string GetSearchStringGorEntryNodes() =>
//        "//div[@class='fit-box']/a[@class='fit-wrapper']";
//    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
//    {
//        //navigate
//        if (pageNode
//                .GetHref()?
//                .Navigate() is not { } detailsPage)
//        {
//            AddWarning(pageNode, "Could not find DetailsDoc");
//            return false;
//        }
//        if (detailsPage
//                .FindNode("//a[@class='photo-blink']")?
//                .GetHref() is not { } imageUri)
//        {
//            AddWarning(detailsPage, "Could not get ImageUri");
//            return false;
//        }

//        //details
//        var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
//        {
//            ImageUri = imageUri,
//            ThumbnailUri = detailsPage
//                .FindNode("//a[@class='photo-blink']/img")?
//                .GetSrc()
//        };
//        source.SetFilenamePrefix(catJob.SiteCategoryName);
//        source.AddTagsFromText(detailsPage.FindNode(
//            "//div[@class='photo-desc']")?.GetText(),
//            ',', '.');

//        //entry
//        if (source.ToWallEntry() is not { } entry)
//            return false;

//        AddEntry(entry, catJob);
//        return true;
//    }

//}