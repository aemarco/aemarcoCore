// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Abyss")]
internal class Abyss : WallpaperCrawlerBasis
{
    private int _queryCount = -1;
    private readonly Uri _uri = new("https://wall.alphacoders.com");
    private readonly Uri _mUri = new("https://mobile.alphacoders.com");
    private readonly HttpClient _httpClient;

    public Abyss(int startPage, int lastPage, bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    {
        _httpClient = new HttpClient();
    }

    private string? _abyssApiKey;
    public void ProvideApiKey(string abyssApiKey)
    {
        _abyssApiKey = abyssApiKey;
    }

    internal override bool IsAvailable =>
        base.IsAvailable &&
        !string.IsNullOrWhiteSpace(_abyssApiKey);




    private List<AbyssCategory> _abyssCats = new();
    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();

        if (string.IsNullOrWhiteSpace(_abyssApiKey))
            return result;

        try
        {
            //check how much query counts are left
            var qc = QueryApi("method=query_count");
            var qcObj = JsonConvert.DeserializeObject<AbyssQueryCount>(qc);
            _queryCount = qcObj!.Counts.MonthCount;



            var str = QueryApi("method=category_list");
            var catResult = JsonConvert.DeserializeObject<AbyssCategoryList>(str);
            if (catResult!.Success)
            {
                _abyssCats = catResult.Categories.ToList();
                foreach (var cat in _abyssCats)
                {
                    var cc = GetContentCategory(cat.Name);
                    if (cc is null)
                        continue;

                    //add api stuff
                    if (_queryCount + 350 < 150000)
                    {
                        //wallpaper from WallpaperAbyss
                        _queryCount += 350;
                        var uri = new Uri(cat.Url);
                        var crawlOffer = CreateCrawlOffer(cat.Name, uri, cc);
                        crawlOffer.CrawlMethod = CrawlMethod.Api;
                        crawlOffer.ReportNumberOfEntriesPerPage(30);
                        result.Add(crawlOffer);
                    }

                    //add classic sites
                    var mUri = new Uri(_mUri, $"/by-category/{cat.Id}");
                    var mOffer = CreateCrawlOffer(cat.Name, mUri, cc);
                    result.Add(mOffer);
                }
            }
        }
        catch
        {
            // ignored
        }

        return result;

    }

    protected override ContentCategory? GetContentCategory(string? categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return DefaultCategory;

        var result = categoryName switch
        {
            "Women" => new ContentCategory(Category.Girls, 1, 29),
            "Vehicles" => new ContentCategory(Category.Vehicle, 0, 0),
            "Video Game" => new ContentCategory(Category.Media_Games, 0, 0),
            "Men" => new ContentCategory(Category.Men, 0, 10),
            "Movie" => new ContentCategory(Category.Media_Movies, 0, 0),
            "Music" => new ContentCategory(Category.Media_Music, 0, 0),
            "TV Show" => new ContentCategory(Category.Media_TVSeries, 0, 0),
            _ => DefaultCategory
        };
        return result;
    }


    //handle CrawlMethods
    protected override bool HandlePage(CrawlOffer catJob)
    {
        return catJob.CrawlMethod switch
        {
            CrawlMethod.Classic => base.HandlePage(catJob),
            CrawlMethod.Api => HandleAbyssApiPage(catJob),
            _ => throw new NotImplementedException($"{catJob.CrawlMethod} not implemented.")
        };
    }

    //api
    private bool HandleAbyssApiPage(CrawlOffer catJob)
    {
        var result = false;

        // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category&id=10&page=10&info_level=2
        var pageString = QueryApi(
            "method=category",
            $"id={_abyssCats.First(x => x.Name == catJob.SiteCategoryName).Id}",
            $"page={catJob.CurrentPage}",
            "check_last=1");
        var page = JsonConvert.DeserializeObject<AbyssWallpaperPage>(pageString);
        if (!page!.Success || !page.Wallpapers.Any())
            return false;


        foreach (var w in page.Wallpapers)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (catJob.ReachedMaximumStreak)
            {
                catJob.ReportEndReached();
                break;
            }

            if (AddWallEntry(catJob, w))
            {
                result = true;
            }
        }

        catJob.ReportPageDone();
        if (page.IsLast)
            catJob.ReportEndReached();

        //valid Page contains minimum 1 valid Entry
        return result;
    }
    private bool AddWallEntry(CrawlOffer catJob, AbyssWallpaper wall)
    {
        // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=wallpaper_info&id=595064
        var wallInfoString = QueryApi("method=wallpaper_info", $"id={wall.Id}");
        var wallInfo = JsonConvert.DeserializeObject<AbyssWallpaperInfoResult>(wallInfoString)
            ?? throw new Exception("Could not parse AbyssWallpaperInfoResult");
        if (!wallInfo.Success)
            return false;

        var tags = wallInfo.Tags.Select(x => x.Name).ToList();
        if (!string.IsNullOrWhiteSpace(wallInfo.Wallpaper.SubCategory))
            tags.Insert(0, wallInfo.Wallpaper.SubCategory);

        var wallEntry = new WallEntry(
            wallInfo.Wallpaper.UrlImage,
            wallInfo.Wallpaper.UrlThumb,
            $"Abyss_{wallInfo.Wallpaper.Category}_{wallInfo.Wallpaper.SubCategory}_{wallInfo.Wallpaper.Id}",
            $".{wallInfo.Wallpaper.FileType}",
            CheckForRealCategory(catJob.Category, tags, wallInfo.Wallpaper),
            catJob.SiteCategoryName,
            tags,
            null);

        if (!wallEntry.IsValid)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }
    private string QueryApi(params string[] parameters)
    {
        // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category_list

        var allParams = parameters.ToList();
        allParams.Insert(0, $"auth={_abyssApiKey}");


        var queryUri = $"{_uri.AbsoluteUri}/api2.0/get.php";
        var call = $"{queryUri}?{string.Join("&", allParams)}";

        var res = _httpClient.GetAsync(call).GetAwaiter().GetResult();
        res.EnsureSuccessStatusCode();
        var result = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        if (string.IsNullOrWhiteSpace(result))
            throw new Exception("Abyss No Query Result");

        return result;

    }
    //api


    //normal
    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://mobile.alphacoders.com/by-category/33?page=1"
        return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}?page={catJob.CurrentPage}");

    }
    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='thumb-container']/div[@class='container-masonry']/div[@class='item-element']/a";
    }
    protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
    {
        var source = new WallEntrySource(_mUri, node, catJob.SiteCategoryName);

        //docs
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }

        //details
        source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='img-full-size']", "src");
        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }
        var thumbUrl = source.GetSubNodeAttribute(source.RootNode, "src", "./img[@class='img-responsive']");
        if (thumbUrl is not null)
            source.ThumbnailUri = new Uri(thumbUrl);

        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);


        source.Tags = source.GetTagsFromNode(source.RootNode, "title", "./img[@class='img-responsive']");
        source.ContentCategory = CheckForRealCategory(catJob.Category, source.Tags, null);


        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }
    //normal



    private ContentCategory CheckForRealCategory(
        ContentCategory cat,
        List<string> tags,
        AbyssWallpaperInfo? info)
    {
        //api walls
        if (info is not null)
        {
            if (info.Category == "Women")
            {
                switch (info.SubCategory)
                {
                    case "Asian":
                        return new ContentCategory(Category.Girls_Asian, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                    case "Girls & Cars":
                        return new ContentCategory(Category.Girls_Cars, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                    case "Cosplay":
                        return new ContentCategory(Category.Girls_Cosplay, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                    case "Girls & Motorcycles":
                        return new ContentCategory(Category.Girls_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                    case "Bikini":
                        return new ContentCategory(Category.Girls_Beaches, 20, cat.SuggestedMaxAdultLevel);

                    case "Model":
                        return new ContentCategory(Category.Girls_Celebrities, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                    case "Girls & Guns":
                        return new ContentCategory(Category.Girls_Guns, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                }
            }
            else if (info.Category == "Vehicles")
            {
                switch (info.SubCategory)
                {
                    case "Motorcycle":
                        return new ContentCategory(Category.Vehicle_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                    case "Aircraft":
                        return new ContentCategory(Category.Vehicle_Planes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                }

            }
        }

        //all walls
        if (tags.Any())
        {
            if (cat.MainCategory == "Girls")
            {
                if (tags.Any(x => x.ToLower().Contains("asian")))
                    return new ContentCategory(Category.Girls_Asian, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & cars")))
                    return new ContentCategory(Category.Girls_Cars, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("cosplay")))
                    return new ContentCategory(Category.Girls_Cosplay, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & motorcycles")))
                    return new ContentCategory(Category.Girls_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("bikini")))
                    return new ContentCategory(Category.Girls_Beaches, 20, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("model")))
                    return new ContentCategory(Category.Girls_Celebrities, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & guns")))
                    return new ContentCategory(Category.Girls_Guns, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
            }
            else if (cat.MainCategory == "Vehicles")
            {
                if (tags.Any(x => x.ToLower().Contains("motorcycle")))
                    return new ContentCategory(Category.Vehicle_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("aircraft")))
                    return new ContentCategory(Category.Vehicle_Planes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
            }
        }

        return cat;
    }



}





public class AbyssCategoryList
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("categories")]
    public AbyssCategory[] Categories { get; set; } = null!;
}

public class AbyssCategory
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("count")]
    public int Count { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; } = null!;
}





public class AbyssQueryCount
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    [JsonProperty("counts")]
    public AbyssCounts Counts { get; set; } = null!;
}

public class AbyssCounts
{
    [JsonProperty("month_count")]
    public int MonthCount { get; set; }
    [JsonProperty("month_price")]
    public int MonthPrice { get; set; }
    [JsonProperty("last_month_count")]
    public int LastMonthCount { get; set; }
    [JsonProperty("last_month_price")]
    public int LastMonthPrice { get; set; }
}



public class AbyssWallpaperPage
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    [JsonProperty("wallpapers")]
    public AbyssWallpaper[] Wallpapers { get; set; } = null!;
    [JsonProperty("is_last")]
    public bool IsLast { get; set; }
}

public class AbyssWallpaper
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;
    [JsonProperty("width")]
    public string Width { get; set; } = null!;
    [JsonProperty("height")]
    public string Height { get; set; } = null!;
    [JsonProperty("file_type")]
    public string FileType { get; set; } = null!;
    [JsonProperty("file_size")]
    public string FileSize { get; set; } = null!;
    [JsonProperty("url_image")]
    public string UrlImage { get; set; } = null!;
    [JsonProperty("url_thumb")]
    public string UrlThumb { get; set; } = null!;
    [JsonProperty("url_page")]
    public string UrlPage { get; set; } = null!;
}




public class AbyssWallpaperInfoResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    [JsonProperty("wallpaper")]
    public AbyssWallpaperInfo Wallpaper { get; set; } = null!;
    [JsonProperty("tags")]
    public AbyssTag[] Tags { get; set; } = null!;
}

public class AbyssWallpaperInfo
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
    [JsonProperty("featured")]
    public object Featured { get; set; } = null!;
    [JsonProperty("width")]
    public string Width { get; set; } = null!;
    [JsonProperty("height")]
    public string Height { get; set; } = null!;
    [JsonProperty("file_type")]
    public string FileType { get; set; } = null!;
    [JsonProperty("file_size")]
    public string FileSize { get; set; } = null!;
    [JsonProperty("url_image")]
    public string UrlImage { get; set; } = null!;
    [JsonProperty("url_thumb")]
    public string UrlThumb { get; set; } = null!;
    [JsonProperty("url_page")]
    public string UrlPage { get; set; } = null!;
    [JsonProperty("category")]
    public string Category { get; set; } = null!;
    [JsonProperty("category_id")]
    public string CategoryId { get; set; } = null!;
    [JsonProperty("sub_category")]
    public string SubCategory { get; set; } = null!;
    [JsonProperty("sub_category_id")]
    public string SubCategoryId { get; set; } = null!;
    [JsonProperty("user_name")]
    public string UserName { get; set; } = null!;
    [JsonProperty("user_id")]
    public string UserId { get; set; } = null!;
    [JsonProperty("collection")]
    public object Collection { get; set; } = null!;
    [JsonProperty("collection_id")]
    public int CollectionId { get; set; }
    [JsonProperty("group")]
    public object Group { get; set; } = null!;
    [JsonProperty("group_id")]
    public int GroupId { get; set; }
}

public class AbyssTag
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
}