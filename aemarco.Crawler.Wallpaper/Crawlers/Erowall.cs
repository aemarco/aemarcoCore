// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;


[Crawler("Erowall")]
internal partial class Erowall : WallpaperCrawlerBasis
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

        var mainPage = new PageUri(_uri).Navigate();
        var catNodes = mainPage.FindNodes("//ul[@class='m']/li[@class='m']/a");
        foreach (var catNode in catNodes)
        {
            //z.B. "#brunette"
            var catName = catNode.GetText();
            if (string.IsNullOrEmpty(catName) || !catName.StartsWith('#'))
                continue;

            //z.B. "brunette"
            catName = catName[1..];
            if (string.IsNullOrEmpty(catName))
                continue;

            //z.B. "Brunette"
            catName = $"{char.ToUpper(catName[0])}{catName[1..]}";

            var cat = GetContentCategory(catName);
            if (cat is null)
                continue;

            //z.B. "/search/brunette/" => "teg/brunette/" => "https://erowall.com/teg/brunette/"
            if (catNode.GetHref(x =>
                    x.Replace("search", "teg")) is not { } uri)
                continue;

            result.Add(CreateCrawlOffer(catName, uri, cat));
        }

        return result;

    }
    //z.B. "https://erowall.com/teg/brunette/page/1" 
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringGorEntryNodes() =>
        "//div[@class='wpmini']/a";
    protected override ContentCategory DefaultCategory => new(Category.Girls);
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
        if (pageNode
                .GetHref(x =>
                    MatchNumberPath().Match(x) is { Success: true } match // z.B. "24741"
                        ? $"/wallpapers/original/{match.Groups[1].Value}.jpg"
                        : null) is not { } imageUri)
        {
            AddWarning(pageNode, "Could not get ImageUri");
            return false;
        }


        //details
        var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = detailsPage
                .FindNode("//div[@class='view-left']/a/img")?
                .GetSrc()
        };
        source.SetFilenamePrefix(catJob.SiteCategoryName);
        source.AddTagsFromText(pageNode.GetAttribute("title"));


        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }


    [GeneratedRegex(@"/(\d+)/$")]
    private static partial Regex MatchNumberPath();
}