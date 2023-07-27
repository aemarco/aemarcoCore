// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Wallpaperscraft")]
internal class WallCraft : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://wallpaperscraft.com");
    public WallCraft(
        int startPage,
        int lastPage,
        bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();

        var mainPage = new PageUri(_uri).Navigate(750, 2500);
        var catNodes = mainPage.FindNodes("//ul[@class='filters__list JS-Filters']/li/a");
        foreach (var catNode in catNodes)
        {

            var text = new PageNode(catNode, catNode.Node.FirstChild).GetText();
            if (GetContentCategory(text) is not { } cat)
                continue;

            if (catNode.GetHref(x => $"{x}/") is not { } href)
                continue;

            result.Add(CreateCrawlOffer(text, href, cat));
        }

        return result;

    }
    //z.B. "https://wallpaperscraft.com/catalog/3d/date"
    //z.B. "https://wallpaperscraft.com/catalog/3d/date/page2"     
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CurrentPage == 1
            ? catJob.CategoryUri.WithHref("date")
            : catJob.CategoryUri.WithHref($"date/page{catJob.CurrentPage}");
    protected override string GetSearchStringGorEntryNodes() =>
        "//li[@class='wallpapers__item']";
    protected override ContentCategory? GetContentCategory(string? categoryName)
    {
        return categoryName switch
        {
            "3D" => new ContentCategory(Category.Fantasy_3D, 0, 0),
            "Abstract" => new ContentCategory(Category.Fantasy_Abstract, 0, 0),
            "Animals" => new ContentCategory(Category.Hobbies_Animals, 0, 0),
            "Anime" => new ContentCategory(Category.Fantasy_Anime, 0, 0),
            "Art" => new ContentCategory(Category.Fantasy_Art, 0, 0),
            //Black
            //Black and white
            "Cars" => new ContentCategory(Category.Vehicle_Cars, 0, 0),
            "City" => new ContentCategory(Category.Environment_City, 0, 0),
            //Dark
            "Fantasy" => new ContentCategory(Category.Fantasy, 0, 0),
            "Flowers" => new ContentCategory(Category.Environment_Flowers, 0, 0),
            "Food" => new ContentCategory(Category.Hobbies_Food, 0, 0),
            "Holidays" => new ContentCategory(Category.Other_Holidays, 0, 0),
            //Love
            "Macro" => new ContentCategory(Category.Environment_Macro, 0, 0),
            //Minimalism
            "Motorcycles" => new ContentCategory(Category.Vehicle_Bikes, 0, 0),
            "Music" => new ContentCategory(Category.Media_Music, 0, 0),
            "Nature" => new ContentCategory(Category.Environment_Landscape, 0, 0),
            "Other" => new ContentCategory(Category.Other, 0, 0),
            "Space" => new ContentCategory(Category.Environment_Space, 0, 0),
            "Sport" => new ContentCategory(Category.Hobbies_Sport, 0, 0),
            "Technologies" => new ContentCategory(Category.Hobbies_HiTech, 0, 0),
            "Textures" => new ContentCategory(Category.Other_Textures, 0, 0),
            "Vector" => new ContentCategory(Category.Fantasy_Vector, 0, 0),
            "Words" => new ContentCategory(Category.Other_Words, 0, 0),


            // gone "Movies" => new ContentCategory(Category.Media_Movies, 0, 0),
            // gone "TV Series" => new ContentCategory(Category.Media_TVSeries, 0, 0),
            // gone "Games" => new ContentCategory(Category.Media_Games, 0, 0),
            _ => DefaultCategory
        };
    }
    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        //navigate
        if (pageNode
                .FindNode("./a")?
                .GetHref()?
                .Navigate(750, 2500) is not { } detailsPage)
        {
            AddWarning(pageNode, "Could not find DetailsDoc");
            return false;
        }
        if (detailsPage
                .FindNode("//div[@class='wallpaper-table__row']/span[@class='wallpaper-table__cell']/a")?
                .GetHref()?
                .Navigate(750, 2500) is not { } downloadPage)
        {
            AddWarning(detailsPage, "Could not find DownloadDoc");
            return false;
        }
        if (downloadPage
                .FindNode("//img[@class='wallpaper__image']")?
                .GetSrc() is not { } imageUri)
        {
            AddWarning(downloadPage, "Could not get ImageUri");
            return false;
        }

        //details
        var source = new WallEntrySource(_uri, pageNode, catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = imageUri,
            ThumbnailUri = downloadPage
                .FindNode("//img[@class='wallpaper__image']")?
                .GetSrc()
        };
        source.SetFilenamePrefix(catJob.SiteCategoryName);
        source.AddTagsFromInnerTexts(downloadPage.FindNodes("//div[@class='wallpaper__tags']/a"));

        //entry
        if (source.ToWallEntry() is not { } entry)
            return false;

        AddEntry(entry, catJob);
        return true;
    }

}