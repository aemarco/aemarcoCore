// ReSharper disable UnusedType.Global
using aemarco.Crawler.Extensions;

namespace aemarco.Crawler.Wallpaper.Crawlers;

[Crawler("Pornpics")]
internal class Pornpics : HtmlCrawlerBasis
{

    private readonly Uri _uri = new("https://www.pornpics.com");
    public Pornpics(
        IHtmlClient htmlClient,
        ILogger<Pornpics> logger)
        : base(htmlClient, logger)
    { }

    protected override bool FirstPageOnly => true;

    protected override async Task<List<CrawlOffer>> GetCrawlOffers()
    {
        //the home page is a tile per category, z.B. "Teen" -> "https://www.pornpics.com/teens/"
        var result = new List<CrawlOffer>();
        if (await Navigate(new PageUri(_uri)) is { } homePage)
        {
            foreach (var tile in homePage.FindNodes(ThumbNodes))
            {
                if (tile.FindNode("./span[@class='h2']")?.GetText() is not { Length: > 0 } cat ||
                    tile.GetHref() is not { } catPage)
                    continue;
                _siteCategories.TryAdd(cat, cat);
                if (GetContentCategory(cat) is { } cc)
                    result.Add(CreateCrawlOffer(cat, catPage.WithHref("recent/"), cc));
            }
        }

        //categories which still work, but have no tile (checked)
        foreach (var cat in HiddenCategories.Where(x => _siteCategories.TryAdd(x, x)))
        {
            if (GetContentCategory(cat) is { } cc)
                result.Add(CreateCrawlOffer(cat, new PageUri(new Uri(_uri, $"/{cat.ToLowerInvariant().Replace(" ", "-")}/recent/")), cc));
        }
        return result;
    }
    //all the site's category names (ignored ones too), to map a gallery's by the tile's spelling
    private readonly Dictionary<string, string> _siteCategories = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] HiddenCategories =
    [
        "Asshole", "BBC", "Big Clit", "British", "Butt Plug", "Cameltoe", "Casting", "Cuckold", "Czech", "Dress",
        "French", "German", "Goth", "Gym", "Indian", "Italian", "Leather", "Russian", "Twins", "Vintage"
    ];
    //the first of a listing or gallery is "thumbwook thumbwook_static", so not by the whole class
    private const string ThumbNodes = "//li[contains(concat(' ', @class, ' '), ' thumbwook ')]/a";
    //z.B. "https://www.pornpics.com/asian/recent/" --> only 1 page
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri;
    protected override string GetSearchStringForEntryNodes() =>
        ThumbNodes;
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override ContentCategory? GetContentCategory(string? categoryName) =>
        categoryName switch
        {
            //"AI" on its tile, "Ai" in the categories of a gallery
            "AI" or "Ai" => new ContentCategory(Category.Girls_Ai),
            "Fetish" => new ContentCategory(Category.Girls_Fetish),
            "Hentai" => new ContentCategory(Category.Girls_Fantasy),
            "Tribbing" => new ContentCategory(Category.Girls_Lesbians),
            "Anal" or "Anal Gape" or "Ass Fucking" or "Ass Licking" or "Ball Licking" or "Big Cock" or
                "Blowbang" or "Bukkake" or "Cowgirl" or "Creampie" or "Cum In Mouth" or "Cum In Pussy" or "Cum Swapping" or
                "Cumshot" or "Deepthroat" or "Double Penetration" or "Facesitting" or "Facial" or "Fisting" or "Footjob" or
                "Gangbang" or "Groupsex" or "Handjob" or "Interracial" or "Missionary" or "Orgy" or "POV" or
                "Pussy Licking" or "Strapon" or "Threesome" or "Titjob" or
                "Old Man" => new ContentCategory(Category.Girls_Hardcore),
            //"Secretary" and "Big Clit" are no acts, so Girls (the default)

            //"Family" is mixed, so Girls (the default)
            //Blowjob, Lesbian, Hardcore, Homemade, Gloryhole, Christmas, Blindfold, MILF, ... come with the shared ones
            _ => base.GetContentCategory(categoryName)
        };

    //a gallery is in several categories, the most explicit one is its category;
    //Fantasy before Ai (all Hentai galleries are Ai too), Lingerie before Selfies and Amateur (else it never wins)
    private static readonly Category[] Precedence =
    [
        Category.Girls_Fantasy,
        Category.Girls_Ai,
        Category.Girls_Gloryhole,
        Category.Girls_Blowjob,
        Category.Girls_Hardcore,
        Category.Girls_Lesbians,
        Category.Girls_Fetish,
        Category.Girls_Cosplay,
        Category.Girls_Holidays,
        Category.Girls_Lingerie,
        Category.Girls_Selfies,
        Category.Girls_Amateur,
        Category.Girls_Beaches,
        Category.Girls_Asian
    ];
    //null leaves the gallery out (one of its categories is ignored)
    private ContentCategory? GetGalleryCategory(IReadOnlyList<string> categories)
    {
        //a gallery's spelling may differ from the tile's ("Ai" vs "AI"), the tile's is mapped
        var mapped = categories
            .Select(x => _siteCategories.GetValueOrDefault(x, x))
            .Select(GetContentCategory)
            .ToList();
        if (mapped.Any(x => x is null))
            return null;
        return Precedence
            .Select(p => mapped.FirstOrDefault(m => m!.Category == p.ToString()))
            .FirstOrDefault(x => x is not null) ?? DefaultCategory;
    }
    protected override async Task<bool> AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //navigate
        if (await Navigate(pageNode.GetHref()) is not { } albumPage)
        {
            AddWarning(pageNode, "Could not find AlbumDoc");
            return false;
        }
        if (pageNode
                .FindNode("./img")?
                .GetAttribute("alt") is not { Length: > 0 } albumName)
        {
            AddWarning(pageNode, "Could not find AlbumName");
            return false;
        }

        //z.B. "Japanese, Asian, Creampie, ..." -> Hardcore, so only the Hardcore offers keep it, the others leave it to them
        //all of them are in the hidden list of the suggest box, the gallery shows only the first 10
        var categories = albumPage
            .FindNode("//div[contains(concat(' ', @class, ' '), ' suggest-2_source-list__cat ')]")?
            .GetText()
            .SplitList()
            .ToList() ?? [];
        if (categories.Count == 0)
            categories = albumPage
                .FindNodes("//div[@class='gallery-info__item tags'][span[@class='gallery-info__title']='Categories:']/div[@class='gallery-info__content']/a/span")
                .Select(x => x.GetText())
                .ToList();
        if (categories.Count > 0 &&
            GetGalleryCategory(categories)?.Category != catJob.Category.Category)
            return true;

        List<Tag>? tags = null;

        var album = new AlbumEntry(albumName);
        var entryNodes = albumPage.FindNodes(ThumbNodes);
        foreach (var entryNode in entryNodes)
        {
            //so that we do this work only once
            if (tags is null)
            {
                var tagSource = new WallEntrySource(catJob.Category, catJob.SiteCategoryName);
                tagSource.AddTagsFromText(catJob.SiteCategoryName);
                tagSource.Tags.AddRange(categories.Select(x => new Tag(x)));
                tagSource.AddTagsFromInnerTexts(albumPage
                    .FindNodes("//div[@class='gallery-info__item tags']/div/a/span"));
                tagSource.AddTagsFromInnerTexts(albumPage
                    .FindNodes("//div[@class='gallery-info__item']/div/a/span"));
                tags = tagSource.Tags;

            }

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
                ImageUri = imageUri,
                Tags = tags
            };

            //entry
            if (source.ToWallEntry() is not { } entry)
                continue;

            album.Entries.Add(entry);
        }

        if (album.Entries.Count == 0)
            return false;

        AddAlbum(album, catJob);
        return true;
    }


}