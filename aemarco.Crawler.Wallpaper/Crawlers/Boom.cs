// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

//the subdomain of Boombo with single pics, a site of its own
[Crawler("Boom")]
internal class Boom : HtmlCrawlerBasis
{

    private readonly IBoomClient _boomClient;
    public Boom(
        IHtmlClient htmlClient,
        IBoomClient boomClient,
        ILogger<Boom> logger)
        : base(htmlClient, logger)
    {
        _boomClient = boomClient;
    }

    //a fixed order, but not by date (checked: no date, id or popularity order; robots.txt disallows sorting)
    protected override bool ListsNewestFirst => false;

    private readonly (string Name, Uri Uri, string Extension, Category Category)[] _categories =
    [
        ("Pics", new Uri("https://boom.boombo.biz/category/pics/"), ".jpg", Category.Girls),
        ("Gifs", new Uri("https://boom.boombo.biz/category/gifs/"), ".gif", Category.Girls),
        ("Sexy arts", new Uri("https://boom.boombo.biz/category/sexy-arts/"), ".jpg", Category.Girls_Fantasy)
    ];

    protected override Task<List<CrawlOffer>> GetCrawlOffers() =>
        Task.FromResult(_categories
            .Select(x => CreateCrawlOffer(x.Name, new PageUri(x.Uri), new ContentCategory(x.Category)))
            .ToList());
    //z.B. "https://boom.boombo.biz/category/pics/page/2", a page beyond the last one has no entries
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringForEntryNodes() =>
        "//article[contains(@class,'card-wall')]";
    protected override async Task<bool> AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var imageNode = pageNode.FindNode(".//a[@class='card-wall-link']/img");
        //z.B. ".../uploads/wallpapers/2026/11/083/pics-golaya-masha-belova-31-55d3fe3a-cba33b18-thumb.webp"
        if (imageNode?.GetSrc() is not { } thumbUri ||
            !thumbUri.Uri.AbsoluteUri.EndsWith("-thumb.webp", StringComparison.Ordinal))
        {
            AddWarning(pageNode, "Could not get ThumbnailUri");
            return false;
        }
        //the full image is the thumb's name with the category's extension (checked)
        var extension = _categories.First(x => x.Name == catJob.SiteCategoryName).Extension;
        var imageUri = new PageUri(new Uri(thumbUri.Uri.AbsoluteUri[..^"-thumb.webp".Length] + extension));

        var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = thumbUri
        };
        //z.B. "Maria Kapustinskaya Naked on Striped Blanket", wallpapers have no title, so it is the first tag
        //(one tag, titles may contain commas)
        if (pageNode.FindNode(".//span[@class='card-wall-title']")?.GetText() is { Length: > 0 } title)
            source.Tags.Add(new Tag(title));

        //the site's tags (z.B. "Sunny Leone") only come with the details, a call per wall, so for new walls only
        if (!IsKnown(imageUri.Uri.AbsoluteUri, catJob))
        {
            if (!int.TryParse(pageNode.GetAttribute("data-wallpaper-id"), out var id))
            {
                AddWarning(pageNode, "Could not get WallpaperId");
                return false;
            }
            if (await _boomClient.GetWallpaper(id, CancellationToken) is not { } details)
            {
                AddWarning(pageNode, $"Wallpaper {id} is gone");
                return false;
            }
            source.Tags.AddRange(details.Tags.Select(x => new Tag(x.Name)));
        }

        //z.B. "Maria Kapustinskaya Naked on Striped Blanket — Erotica", the collection after the dash
        if (WebUtility.HtmlDecode(imageNode.GetAttribute("alt")) is { } alt &&
            alt.LastIndexOf(" — ", StringComparison.Ordinal) is var dash and >= 0)
            source.AddTagsFromText(alt[(dash + " — ".Length)..]);

        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }

}
