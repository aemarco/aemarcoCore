//// ReSharper disable UnusedType.Global
//namespace aemarco.Crawler.Wallpaper.Crawlers;

//[Crawler("Zoompussy")]
//internal class Zoompussy : WallpaperCrawlerBasis
//{

//    private readonly Uri _uri = new("https://zoompussy.com");
//    public Zoompussy(
//        int startPage,
//        int lastPage,
//        bool onlyNews)
//        : base(startPage, lastPage, onlyNews)
//    { }

//    protected override List<CrawlOffer> GetCrawlsOffers()
//    {
//        var cats = new List<string>
//        {
//            "asian",
//            "ass",
//            "bikini",
//            "blonde",
//            "boobs",
//            "ebony",
//            "tits",
//            "brunette",
//            "legs",
//            "lingerie",
//            "models",
//            "naked",
//            "nude",
//            "pussy",
//            "redhead",
//            "stockings"
//        };

//        var result = cats
//            .Select(cat =>
//                GetContentCategory(cat) is { } cc
//                    ? CreateCrawlOffer(cat, new PageUri(new Uri(_uri, $"search/{cat}/")), cc)
//                    : null)
//            .OfType<CrawlOffer>()
//            .ToList();

//        return result;
//    }
//    //z.B. "http://zoompussy.com/search/asian/page/1/"
//    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
//        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
//    protected override string GetSearchStringGorEntryNodes() =>
//        "//li/div[@class='thumb']/a";
//    protected override ContentCategory DefaultCategory => new(Category.Girls);
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
//                .FindNode("//div[@id='post_content']/blockquote/a")?
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
//                .FindNode("//div[@id='post_content']/blockquote/a/img")?
//                .GetSrc()
//        };
//        source.SetFilenamePrefix(catJob.SiteCategoryName);
//        source.AddTagsFromInnerTexts(
//            detailsPage
//                .FindNodes("//div[@class='post_z']/a")
//                .Where(x => x.GetHref()?.Uri is { } link && link.Host == _uri.Host));

//        //entry
//        if (source.ToWallEntry() is not { } entry)
//            return false;

//        AddEntry(entry, catJob);
//        return true;
//    }
//}

////damn.... no fun without referer --> special treatment needed :(
////source.DownloadWithReferer(wallEntry, $"{_uri.AbsoluteUri}{wallEntry.FileName.ToLower()}");
////if (!wallEntry.IsValid || string.IsNullOrWhiteSpace(wallEntry.FileContentAsBase64String)) 
////    return false;