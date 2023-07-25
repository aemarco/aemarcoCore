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
    {

    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();

        //main page
        var doc = HtmlHelper.GetHtmlDocument(_uri, 700, 2500);
        var nodes = doc.DocumentNode.SelectNodes("//ul[@class='filters__list JS-Filters']/li/a");
        if (nodes == null) return result;
        foreach (var node in nodes)
        {
            var text = WebUtility.HtmlDecode(node.FirstChild.InnerText).Trim();
            var cat = GetContentCategory(text);
            if (cat is null)
            {
                continue;
            }

            var href = node.Attributes["href"]?.Value;
            if (string.IsNullOrEmpty(href) || href.StartsWith("http"))
            {
                continue;
            }
            var uri = new Uri(_uri, href);

            result.Add(CreateCrawlOffer(text, uri, cat));
        }
        return result;

    }
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        if (catJob.CurrentPage == 1)
        {
            //z.B. "https://wallpaperscraft.com/catalog/girls/date"
            //return $"{catJob.CategoryUri.AbsoluteUri}/date";

            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/date");
        }

        //z.B. "https://wallpaperscraft.com/catalog/girls/date/page2"       
        //return $"{catJob.CategoryUri.AbsoluteUri}/date/page{catJob.CurrentPage}";
        return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/date/page{catJob.CurrentPage}");
    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//li[@class='wallpapers__item']";
    }
    protected override ContentCategory? GetContentCategory(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return null;

        switch (categoryName)
        {
            case "3D":
                return new ContentCategory(Category.Fantasy_3D, 0, 0);
            case "Abstract":
                return new ContentCategory(Category.Fantasy_Abstract, 0, 0);
            case "Animals":
                return new ContentCategory(Category.Hobbies_Animals, 0, 0);
            case "Anime":
                return new ContentCategory(Category.Fantasy_Anime, 0, 0);
            case "Art":
                return new ContentCategory(Category.Fantasy_Art, 0, 0);
            case "Cars":
                return new ContentCategory(Category.Vehicle_Cars, 0, 0);
            case "City":
                return new ContentCategory(Category.Environment_City, 0, 0);
            case "Fantasy":
                return new ContentCategory(Category.Fantasy, 0, 0);
            case "Flowers":
                return new ContentCategory(Category.Environment_Flowers, 0, 0);
            case "Food":
                return new ContentCategory(Category.Hobbies_Food, 0, 0);
            case "Games":
                return new ContentCategory(Category.Media_Games, 0, 0);
            case "Technologies":
                return new ContentCategory(Category.Hobbies_HiTech, 0, 0);
            case "Holidays":
                return new ContentCategory(Category.Other_Holidays, 0, 0);
            case "Macro":
                return new ContentCategory(Category.Environment_Macro, 0, 0);
            case "Motorcycles":
                return new ContentCategory(Category.Vehicle_Bikes, 0, 0);
            case "Movies":
                return new ContentCategory(Category.Media_Movies, 0, 0);
            case "Music":
                return new ContentCategory(Category.Media_Music, 0, 0);
            case "Nature":
                return new ContentCategory(Category.Environment_Landscape, 0, 0);
            case "Other":
                return new ContentCategory(Category.Other, 0, 0);
            case "Space":
                return new ContentCategory(Category.Environment_Space, 0, 0);
            case "Sport":
                return new ContentCategory(Category.Hobbies_Sport, 0, 0);
            case "Textures":
                return new ContentCategory(Category.Other_Textures, 0, 0);
            case "TV Series":
                return new ContentCategory(Category.Media_TVSeries, 0, 0);
            case "Vector":
                return new ContentCategory(Category.Fantasy_Vector, 0, 0);
            case "Words":
                return new ContentCategory(Category.Other_Words, 0, 0);
        }
        return DefaultCategory;
    }


    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;


        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode("./a", 700, 2500);
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }
        source.DownloadDoc = source.GetChildDocumentFromDocument(source.DetailsDoc, "//div[@class='wallpaper-table__row']/span[@class='wallpaper-table__cell']/a", 700, 2500);
        if (source.DownloadDoc is null)
        {
            AddWarning($"Could not read DownloadDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//img[@class='wallpaper__image']", "src");

        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DownloadDoc.DocumentNode.InnerHtml}");
            return false;
        }
        source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='wallpaper__image']", "src");
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
        source.ContentCategory = catJob.Category;
        source.Tags = source.GetTagsFromNodes(source.DownloadDoc, "//div[@class='wallpaper__tags']/a", x => WebUtility.HtmlDecode(x.InnerText).Trim());


        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

}