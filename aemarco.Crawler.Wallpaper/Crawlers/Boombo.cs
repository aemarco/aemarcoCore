// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[Crawler("Boombo")]
internal class Boombo : HtmlCrawlerBasis
{

    private readonly Uri _uri = new("https://boombo.biz");
    public Boombo(
        IHtmlClient htmlClient,
        ILogger<Boombo> logger)
        : base(htmlClient, logger)
    { }

    //the english part is two archives, an album's category comes from the folder of its url,
    //e.g. "/en/porn_photos/72474-....html" or "/en/nude/bondage/32621-....html";
    //a folder not listed is Girls, null leaves the album out
    //all folders of "/en/" (sampled over its whole range)
    private static readonly Dictionary<string, ContentCategory?> SectionFolders = new()
    {
        ["naked-girls"] = new ContentCategory(Category.Girls),
        ["big-tits"] = new ContentCategory(Category.Girls),
        ["pussy"] = new ContentCategory(Category.Girls),
        ["tits"] = new ContentCategory(Category.Girls),
        ["porn_photos"] = new ContentCategory(Category.Girls_Hardcore),
        ["anal"] = new ContentCategory(Category.Girls_Hardcore)
    };
    //each category here is an offer paging the whole archive, so only frequent folders (1.5% and more of the albums),
    //rare ones (lesbian, beach, blowjob, < 1%) stay Girls, the categories of the russian part cover them
    private static readonly Dictionary<string, ContentCategory?> NudeFolders = new()
    {
        ["asian"] = new ContentCategory(Category.Girls_Asian),
        ["bondage"] = new ContentCategory(Category.Girls_Fetish),
        ["bdsm"] = new ContentCategory(Category.Girls_Fetish),
        ["amateur"] = new ContentCategory(Category.Girls_Amateur),
        ["hardcore"] = new ContentCategory(Category.Girls_Hardcore),
        ["anal"] = new ContentCategory(Category.Girls_Hardcore),
        ["facial"] = new ContentCategory(Category.Girls_Hardcore),
        ["cumshot"] = new ContentCategory(Category.Girls_Hardcore),
        ["threesome"] = new ContentCategory(Category.Girls_Hardcore),
        ["handjob"] = new ContentCategory(Category.Girls_Hardcore),
        ["cowgirl"] = new ContentCategory(Category.Girls_Hardcore),
        ["rimming"] = new ContentCategory(Category.Girls_Hardcore),
        ["swingers"] = new ContentCategory(Category.Girls_Hardcore),

        //some to be ignored
        ["bbw"] = null,
        ["chubby"] = null,
        ["mature"] = null,
        ["milf"] = null,
        ["housewife"] = null,
        ["granny"] = null,
        ["party"] = null
    };
    //one offer per archive and category, each keeps only the albums of its category,
    //so filtering by category runs exactly the offers delivering it
    private readonly (string Name, Uri Uri, Dictionary<string, ContentCategory?> Folders)[] _archives =
    [
        //all english sections but "nude"
        ("Nude Pics", new Uri("https://boombo.biz/en/"), SectionFolders),
        ("Nude Photo", new Uri("https://boombo.biz/en/nude/"), NudeFolders)
    ];
    //sections without categories, the albums of all sections are distinct (checked)
    private readonly (string Name, Uri Uri)[] _sections =
    [
        ("Naked Girls", new Uri("https://boombo.biz/naked-girls/")),
        ("Hot Girls", new Uri("https://hot.boombo.biz/"))
    ];

    protected override async Task<List<CrawlOffer>> GetCrawlOffers()
    {
        var result = _archives
            .SelectMany(x => x.Folders.Values
                .OfType<ContentCategory>()
                .Append(DefaultCategory)
                .DistinctBy(cc => cc.Category)
                //z.B. "Nude Photo (Fetish)", the Girls one is just "Nude Photo"
                .Select(cc => CreateCrawlOffer(
                    cc.SubCategory is { Length: > 0 } sub ? $"{x.Name} ({sub})" : x.Name,
                    new PageUri(x.Uri),
                    cc)))
            .Concat(_sections
                .Select(x => CreateCrawlOffer(x.Name, new PageUri(x.Uri), DefaultCategory)))
            .ToList();

        if (await Navigate(new PageUri(new Uri(_uri, "en/"))) is not { } mainPage)
            return result;
        //the categories of the mega menu, a link each, e.g. "Anal Sex" -> "https://boombo.biz/en/anal/"
        var catNodes = mainPage.FindNodes("//div[@class='submenu mega-menu']//ul/li/a");

        foreach (var catNode in catNodes)
        {

            //z.B. "Nude photo"
            if (catNode.GetText() is not { Length: > 0 } cat)
                continue;

            //z.B. "/asian-girls/"
            if (catNode.GetHref() is not { } catPage)
                continue;

            //english ones ("Nude Pics", "Anal Sex") are part of the english archives
            if (catPage.Uri.Segments is [_, "en/", ..])
                continue;

            if (GetContentCategory(cat) is not { } cc)
                continue;

            result.Add(CreateCrawlOffer(cat, catPage, cc));
        }

        return result;
    }
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override ContentCategory? GetContentCategory(string? categoryName) =>
        categoryName switch
        {
            "Asian Girls" or "Japanese Girls" => new ContentCategory(Category.Girls_Asian),
            "Beach Girls" => new ContentCategory(Category.Girls_Beaches),
            "BDSM Porn" => new ContentCategory(Category.Girls_Fetish),
            "Nylon Legs" => new ContentCategory(Category.Girls_Lingerie),
            "Interracial Sex" or "Public Sex" or "Porn Pics" => new ContentCategory(Category.Girls_Hardcore),

            //some to be ignored
            "BBW Women" or "Married Women" or "Mature Women" or "Milf Women" or "Nude Men" => null,
            _ => base.GetContentCategory(categoryName)
        };
    //z.B. "https://boombo.biz/en/nude/page/1/"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithHref($"page/{catJob.CurrentPage}");
    protected override string GetSearchStringForEntryNodes() =>
        "//div[@class='fonwall-item']/a";
    protected override async Task<bool> AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //z.B. "https://boombo.biz/en/nude/bondage/32621-randy-moore-lesbian-bondage.html" -> "bondage"
        if (_archives.FirstOrDefault(x => x.Uri == catJob.CategoryUri.Uri) is { Folders: { } folders } &&
            pageNode.GetHref()?.Uri.Segments is [.., var folder, _])
        {
            var folderCategory = folders.TryGetValue(folder.TrimEnd('/'), out var cc) ? cc : DefaultCategory;
            //an album of another category (or left out) is not this offer's, it still counts, so the paging goes on
            if (folderCategory?.Category != catJob.Category.Category)
                return true;
        }

        //navigate
        if (await Navigate(pageNode.GetHref()) is not { } albumPage)
        {
            AddWarning(pageNode, "Could not find AlbumDoc");
            return false;
        }
        if (pageNode
                .FindNode(".//span[@class='fonwall-title']")?
                .GetText() is not { Length: > 0 } albumName)
        {
            AddWarning(pageNode, "Could not find AlbumName");
            return false;
        }


        var album = new AlbumEntry(albumName);
        var entryNodes = albumPage.FindNodes("//a[@class='highslide']");
        foreach (var entryNode in entryNodes)
        {
            //details
            if (entryNode
                    .GetHref() is not { } imageUri)
            {
                AddWarning(entryNode, "Could not get ImageUri");
                continue;
            }
            var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
            {
                AlbumName = albumName,
                ImageUri = imageUri
            };
            source.AddTagsFromText(entryNode.FindNode("./img")?.GetAttribute("alt"));

            //entry
            if (source.ToWallEntry() is not { } entry)
                continue;

            album.Entries.Add(entry);
        }

        var entryNodes2 = albumPage.FindNodes("//div[@class='text']/div/img");
        foreach (var entryNode in entryNodes2)
        {
            //details
            if (entryNode
                    .GetAttributeRef("data-src") is not { } imageUri)
            {
                AddWarning(entryNode, "Could not get ImageUri");
                return false;
            }
            var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
            {
                AlbumName = albumName,
                ImageUri = imageUri
            };
            source.AddTagsFromText(entryNode.GetAttribute("alt"));

            //entry
            if (source.ToWallEntry() is not { } entry)
                continue;

            album.Entries.Add(entry);
        }

        if (album.Entries.Count == 0)
            return false;

        album.Entries.Sort(new WallEntryNumberComparer());
        AddAlbum(album, catJob);
        return true;

    }

}
public partial class WallEntryNumberComparer : IComparer<WallEntry>
{
    public int Compare(WallEntry? x, WallEntry? y) =>
        GetNumber(x).CompareTo(GetNumber(y));


    [GeneratedRegex(@"(\d+)(?!.*\d)")]
    private static partial Regex EndNumberRegex();

    private static int GetNumber(WallEntry? x)
    {
        if (x is null)
            return 0;

        if (EndNumberRegex().Match(x.Url) is not { Success: true } match)
            return 0;

        return int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }
}
