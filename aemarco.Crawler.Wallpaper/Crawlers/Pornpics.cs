// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[Crawler("Pornpics")]
internal class Pornpics : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://www.pornpics.com");
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
        //categories are filled by js :(
        var cats = new List<string>
        {
            "Amateur",
            "Anal", //HC
            "Anal Gape", //HC
            //"Arab",
            "Asian",
            "Ass",
            "Asshole",
            "BBC",
            //"BBW",
            "BDSM",
            "Beach",
            "Big Clit", //HC
            "Big Cock", //HC
            "Big Tits",
            "Bikini",
            "Blonde",
            "Blowjob", //HC
            "Bondage",
            "Boots",
            "British",
            "Brunette",
            "Bukkake", //HC
            "Butt Plug",
            "Cameltoe",


            "Casting",
            //"Chubby",
            "Clothed",
            "Cosplay",
            //"Cougar",
            "Creampie", //HC
            "Cuckold",
            "Cum In Mouth", //HC
            "Cum In Pussy", //HC
            "Cumshot", //HC
            "Curvy",
            "Cute",
            "Czech",
            "Deepthroat", //HC
            "Dildo",
            "Doggy Style",
            "Double Penetration", //HC
            "Dress",
            "Ebony",
            "Face",
            "Facesitting", //HC
            "Facial", //HC
            "Fake Tits",
            "Feet",


            //"Femdom",
            "Fisting", //HC
            "Footjob", //HC
            "French",
            "Gangbang", //HC
            //"Gay",
            "German",
            "Glasses",
            "Goth",
            //"Granny",
            "Gym",
            "Hairy",
            "Handjob", //HC
            "High Heels",
            "Indian",
            "Interracial", //HC
            "Italian",
            "Japanese",
            "Jeans",
            //"Ladyboy",
            "Latex",
            "Latina",
            "Leather",
            "Lesbian", //HC


            "Lingerie",
            "Massage",
            //"Mature",
            //"MILF",
            //"Mom",
            "Natural Tits",
            "Nurse",
            "Office",
            "Old Young",
            "Outdoor",
            "Panties",
            "Pantyhose",
            "PAWG",
            "Petite",
            "Pissing",
            "Pornstar",
            "POV", //HC
            "Pregnant",
            "Public",
            "Pussy",
            "Pussy Licking", //HC
            "Redhead",
            "Russian",
            //"Saggy Tits",


            "Schoolgirl",
            "Secretary", //HC
            "Selfie",
            //"Shemale",
            "Short Hair",
            "Shower",
            "Skinny",
            "Skirt",
            "Socks",
            "Stockings",
            "Tall",
            "Tattoo",
            "Teacher",
            "Teen",
            "Thai",
            "Thong",
            "Threesome", //HC
            "Tiny Tits",
            "Twins",
            "Upskirt",
            "Vintage",
            "Yoga Pants"


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
                tagSource.AddTagsFromText(catJob.SiteCategoryName);
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