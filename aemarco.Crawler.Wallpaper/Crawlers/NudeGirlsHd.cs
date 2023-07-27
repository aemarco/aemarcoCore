// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

/// <summary>
/// The site does not add new content since a long time
/// </summary>
[WallpaperCrawler("NudeGirlsHd")]
internal class NudeGirlsHd : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://nudegirlshd.com");
    public NudeGirlsHd(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {
            CreateCrawlOffer(
                "NudePics",
                new PageUri(_uri),
                new ContentCategory(Category.Girls))
        };
        return result;
    }
    //z.B. "https://nudegirlshd.com/page/2"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringGorEntryNodes() =>
        "//div/a[@class='b-photobox']";
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

        if (detailsPage
                .FindNode("//div[@class='b-card-content js-fit-box js-responzer-box']/a")?
                .GetHref() is not { } imageUri)
        {
            AddWarning(detailsPage, "Could not get ImageUri");
            return false;
        }

        //details
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = pageNode
                .FindNode("//img")?
                .GetSrc()
        };

        //add cat-tags
        source.AddTagsFromInnerTexts(detailsPage.FindNodes("//span[@class='b-photogrid-text']"));
        source.OverrideCategory(CheckForRealCategory(catJob.Category, source.Tags));
        //add tag-tags
        source.AddTagsFromInnerTexts(detailsPage.FindNodes("//a[@class='b-button']/span"));


        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }
    private static ContentCategory CheckForRealCategory(
        ContentCategory cat,
        List<string> tags)
    {
        var entries = new List<(string Cat, int Min, int Max, Category Category)>()
        {
            ("Asian", -1 , -1, Category.Girls_Asian),

            ("Double penetration", 95 , 100, Category.Girls_Hardcore),
            ("Deep penetration", 95, 100, Category.Girls_Hardcore),
            ("Vaginal sex", 95, 100, Category.Girls_Hardcore),
            ("Cowgirl", 95, 100, Category.Girls_Hardcore),
            ("Doggy Style", 95, 100, Category.Girls_Hardcore),

            ("Blowjobs", 90, 94, Category.Girls_Blowjob),
            ("Facial", 90 , 94, Category.Girls_Blowjob),
            ("Deep Throats", 90, 94, Category.Girls_Blowjob),

            ("Lesbians", 80, 89, Category.Girls_Lesbians),
            ("Pussy Licking", 80, 89, Category.Girls),

            ("Fingering", 70, 79, Category.Girls),

            ("Threesomes", 90, 100, Category.Girls_Hardcore),
            ("Group Sex", 90, 100, Category.Girls_Hardcore),
            ("Hardcore", 90, 100, Category.Girls_Hardcore),

            ("Amateur", -1, -1, Category.Girls_Amateur)
        };

        return entries
                   .Where(x => tags.Contains(x.Cat))
                   .Select(x => new ContentCategory(x.Category, x.Min, x.Max))
                   .FirstOrDefault()
               ?? cat;


    }
}
