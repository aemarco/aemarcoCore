﻿// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

//TODO modernize
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
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();
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

        foreach (var cat in cats)
        {
            var cc = GetContentCategory(cat);
            if (cc is null)
            {
                continue;
            }

            var newOffer = CreateCrawlOffer(
                cat,
                new Uri(_uri, $"/recent/{cat.ToLower().Replace(" ", "-")}/")!,
                cc);
            result.Add(newOffer);

            var popularOffer = CreateCrawlOffer(
                cat,
                new Uri(_uri, $"/{cat.ToLower().Replace(" ", "-")}/")!,
                cc);
            result.Add(popularOffer);

        }
        return result;
    }
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://www.pornpics.com/recent/asian/"
        return catJob.CategoryUri;
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//li[@class='thumbwook']/a";
    }

    protected override ContentCategory DefaultCategory => new(Category.Girls);

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;

        var linkToAlbum = node.Attributes["href"]?.Value;
        var albumName = WallEntrySource.GetSubNodeAttrib(node, "alt", "./img");
        if (string.IsNullOrWhiteSpace(albumName)) return false;

        var linkToAlbumUri = new Uri(_uri, linkToAlbum);
        var albumDoc = HtmlHelper.GetHtmlDocument(linkToAlbumUri);

        var entryNodes = albumDoc.DocumentNode.SelectNodes("//li[@class='thumbwook']/a");

        List<string>? tags = null;
        var album = new AlbumEntry(albumName);
        foreach (var entryNode in entryNodes)
        {
            var source = new WallEntrySource(_uri, node, catJob.Category, catJob.SiteCategoryName);

            if (tags is null)
            {
                tags = WallEntrySource.GetTagsFromNodes(
                    albumDoc,
                    "//div[@class='gallery-info__item tags']/div/a/span",
                    x => WebUtility.HtmlDecode(x.InnerText).Trim());

                tags.AddRange(WallEntrySource.GetTagsFromNodes(
                    albumDoc,
                    "//div[@class='gallery-info__item']/div/a/span",
                    x => WebUtility.HtmlDecode(x.InnerText).Trim()));
            }

            //details
            source.ImageUri = new PageUri(new Uri(entryNode.Attributes["href"].Value));
            if (source.ImageUri is null)
            {
                AddWarning($"Could not get ImageUri from node {entryNode.InnerHtml}");
                return false;
            }
            source.ThumbnailUri = source.ImageUri;
            source.Tags = tags;
            source.AlbumName = albumName;

            var wallEntry = source.WallEntry;
            if (wallEntry != null)
            {
                album.Entries.Add(wallEntry);
            }
        }

        if (album.Entries.Any())
        {
            AddAlbum(album, catJob);
        }
        return album.Entries.Any();
    }


}