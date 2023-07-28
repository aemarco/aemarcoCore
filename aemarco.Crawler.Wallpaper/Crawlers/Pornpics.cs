// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Pornpics")]
internal class Pornpics : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://www.pornpics.com/");
    public Pornpics(
#pragma warning disable IDE0060
        // ReSharper disable UnusedParameter.Local
        int startPage,
        int lastPage,
        // ReSharper restore UnusedParameter.Local
#pragma warning restore IDE0060
        bool onlyNews)
        : base(1, 1, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var cats = new List<string>
        {
            "Amateur",
            "Anal", //HC
            "Anal Gape", //HC
            "Asian",
            "Ass",
            "Ass Fucking", //HC
            "Ass Licking", //HC
            "Babe",
            "Ball Licking", //HC
            "Bath",
            //"BBW",
            "Beach",
            "Big Cock", //HC
            "Big Tits",
            "Bikini",
            "Blindfold",
            "Blonde",
            "Blowbang", //HC
            "Blowjob", //HC
            "Bondage",
            "Boots",
            "Brunette",
            "Bukkake", //HC
            "Centerfold",
            //"CFNM",
            "Cheerleader",
            "Christmas",
            "Close Up",
            "Clothed",
            "College",
            "Cosplay",
            //"Cougar",
            "Cowgirl", //HC
            "Creampie", //HC
            "Cum In Mouth", //HC
            "Cum In Pussy", //HC
            "Cumshot", //HC
            "Cum Swapping", //HC
            "Deepthroat", //HC
            "Dildo",
            "Double Penetration", //HC
            "Ebony",
            "European",
            "Face",
            "Facesitting", //HC
            "Facial", //HC
            //"Femdom",
            "Fetish",
            "Fingering",
            "Fisting", //HC
            "Flexible",
            "Foot Fetish",
            "Footjob", //HC
            "Gangbang", //HC
            "Girlfriend",
            "Glamour",
            "Glasses",
            "Gloryhole", //HC
            //"Granny",
            "Groupsex", //HC
            "Gyno", //HC
            "Hairy",
            "Handjob", //HC
            "Hardcore", //HC
            "High Heels",
            "Homemade",
            //"Housewife",
            "Humping",
            "Indian",
            "Interracial", //HC
            "Japanese",
            "Jeans",
            "Kissing", //HC
            //"Ladyboy",
            "Latex",
            "Latina",
            "Legs",
            "Lesbian", //HC
            "Lingerie",
            "Maid",
            "Massage",
            "Masturbation",
            //"Mature",
            //"MILF",
            //"Mom",
            "Nipples",
            "Non Nude",
            "Nurse",
            "Office",
            "Oiled",
            "Orgy", //HC
            "Outdoor",
            "Panties",
            "Pantyhose",
            //"Party",
            //"Pegging",
            "Petite",
            "Piercing",
            "Pissing",
            "Police",
            "Pool",
            "Pornstar",
            "POV", //HC
            "Public",
            "Pussy",
            "Pussy Licking", //HC
            "Reality",
            "Redhead",
            //"Saggy Tits",
            "Schoolgirl",
            "Secretary", //HC
            "Seduction", //HC
            "Self Shot",
            "Shaved",
            //"Shemale",
            "Shorts",
            "Shower",
            "Skinny",
            "Skirt",
            "Smoking",
            "Socks",
            "Spanking",
            "Sports",
            "Spreading",
            "Squirting", //HC
            //"SSBBW",
            "Stockings",
            "Strapon", //HC
            "Stripper",
            "Tall",
            "Tattoo",
            "Teacher",
            "Teen",
            "Thai",
            "Threesome", //HC
            "Tiny Tits",
            "Titjob", //HC
            "Tribbing", //HC
            "Undressing",
            "Uniform",
            "Upskirt",
            "Voyeur",
            "Wedding", //HC
            "Wet",
            //"Wife",
            "Yoga Pants",

        };

        var result = cats
            .Select(cat =>
                GetContentCategory(cat) is { } cc
                    ? CreateCrawlOffer(cat, new PageUri(new Uri(_uri, $"/{cat.ToLower().Replace(" ", "-")}/recent/")), cc)
                    : null)
            .OfType<CrawlOffer>()
            .ToList();
        return result;
    }
    //z.B. "https://www.pornpics.com/asian/recent/" --> only 1 page
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri;
    protected override string GetSearchStringGorEntryNodes() =>
        "//li[@class='thumbwook']/a";
    protected override ContentCategory DefaultCategory => new(Category.Girls);
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //navigate
        if (pageNode
                .GetHref()?
                .Navigate() is not { } albumPage)
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

        List<string>? tags = null;

        var album = new AlbumEntry(albumName);
        var entryNodes = albumPage.FindNodes("//li[@class='thumbwook']/a");
        foreach (var entryNode in entryNodes)
        {
            //so that we do this work only once
            if (tags is null)
            {
                var tagSource = new WallEntrySource(catJob.Category, catJob.SiteCategoryName);
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