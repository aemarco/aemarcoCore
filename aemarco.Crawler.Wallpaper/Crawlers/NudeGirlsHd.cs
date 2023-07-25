// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;


[WallpaperCrawler("NudeGirlsHd")]
internal class NudeGirlsHd : WallpaperCrawlerBasis
{
    private readonly Uri _uri = new("https://nudegirlshd.com");

    public NudeGirlsHd(int startPage, int lastPage, bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    { }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>
        {
            CreateCrawlOffer(
                "NudePics",
                _uri,
                new ContentCategory(Category.Girls))
        };
        return result;
    }

    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://nudegirlshd.com/page/2"
        //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}";
        return new Uri(catJob.CategoryUri, $"/page/{catJob.CurrentPage}");
    }

    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div/a[@class='b-photobox']";
    }

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;
        var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

        //doc
        source.DetailsDoc = source.GetChildDocumentFromRootNode();
        if (source.DetailsDoc is null)
        {
            AddWarning($"Could not read DetailsDoc from node {node.InnerHtml}");
            return false;
        }
        //details
        var thumb = source.GetSubNodeAttribute(node, "src", "//img");
        if (!string.IsNullOrWhiteSpace(thumb))
            source.ThumbnailUri = new Uri(thumb);
        //details
        source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='b-card-content js-fit-box js-responzer-box']/a", "href");


        if (source.ImageUri is null)
        {
            AddWarning($"Could not get ImageUri from node {source.DetailsDoc.DocumentNode.InnerHtml}");
            return false;
        }
        (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);


        source.Tags = new List<string>();
        //add category-tags
        source.Tags.AddRange(
            source.GetTagsFromNodes(source.DetailsDoc, "//span[@class='b-photogrid-text']", x => WebUtility.HtmlDecode(x.InnerText).Trim()));

        source.ContentCategory = CheckForRealCategory(catJob.Category, source.Tags);

        //add tag-tags
        source.Tags.AddRange(
            source.GetTagsFromNodes(source.DetailsDoc, "//a[@class='b-button']/span", x => WebUtility.HtmlDecode(x.InnerText).Trim()));

        var wallEntry = source.WallEntry;
        if (wallEntry == null)
        {
            return false;
        }
        AddEntry(wallEntry, catJob);
        return true;
    }

    private ContentCategory CheckForRealCategory(
        ContentCategory cat,
        List<string> tags)
    {
        var entries = new List<(string Cat, int Min, int Max, Category Category)>()
        {
            ("Asian", -1 , -1, Category.Girls_Asian),

            ("Double penetration", 95 , 100, Category.Girls_Hardcore),
            ("Deep penetration", 95, 100, Category.Girls_Hardcore),
            ("Vaginal sex", 95, 100, Category.Girls_Hardcore),
            ("Cowgirl", 95, 100, Category.Girls_Hardcore),
            ("Doggy Style", 95, 100, Category.Girls_Hardcore),

            ("Blowjobs", 90, 94, Category.Girls_Blowjob),
            ("Facial", 90 , 94, Category.Girls_Blowjob),
            ("Deep Throats", 90, 94, Category.Girls_Blowjob),

            ("Lesbians", 80, 89, Category.Girls_Lesbians),
            ("Pussy Licking", 80, 89, Category.Girls),

            ("Fingering", 70, 79, Category.Girls),

            ("Threesomes", 90, 100, Category.Girls_Hardcore),
            ("Group Sex", 90, 100, Category.Girls_Hardcore),
            ("Hardcore", 90, 100, Category.Girls_Hardcore),
        };

        foreach (var entry in entries)
        {
            if (tags.Contains(entry.Cat))
            {
                return new ContentCategory(entry.Category, entry.Min, entry.Max);
            }
        }
        return cat;
    }
}
