// ReSharper disable UnusedType.Global
namespace aemarco.Crawler.Wallpaper.Crawlers;

/// <summary>
/// wallhaven.cc through its api, available only with an api key (WallCrawlerSettings)
/// </summary>
[Crawler("Wallhaven")]
internal class Wallhaven : WallpaperCrawlerBasis
{

    private readonly WallCrawlerSettings _settings;
    private readonly IWallhavenClient _client;
    public Wallhaven(
        WallCrawlerSettings settings,
        IWallhavenClient client,
        IHtmlClient htmlClient)
        : base(htmlClient)
    {
        _settings = settings;
        _client = client;
    }

    internal override bool IsAvailable => !string.IsNullOrWhiteSpace(_settings.WallhavenApiKey);

    //categories: general/anime/people, purity: sfw/sketchy/nsfw
    protected override Task<List<CrawlOffer>> GetCrawlOffers()
    {
        List<CrawlOffer> result =
        [
            //general covers everything else, it goes to the consumer's default category
            CreateCrawlOffer(
                "General_SFW",
                new PageUri(WallhavenClient.GetSearchUri("100", "100")),
                new ContentCategory(Category.None, 0, 29)),

            CreateCrawlOffer(
                "General_Sketchy",
                new PageUri(WallhavenClient.GetSearchUri("100", "010")),
                new ContentCategory(Category.None, 30, 39)),

            CreateCrawlOffer(
                "General_NSFW",
                new PageUri(WallhavenClient.GetSearchUri("100", "001")),
                new ContentCategory(Category.None, 40, 69)),

            CreateCrawlOffer(
                "Anime_SFW",
                new PageUri(WallhavenClient.GetSearchUri("010", "100")),
                new ContentCategory(Category.Girls_Fantasy, 10, 29)),

            CreateCrawlOffer(
                "Anime_Sketchy",
                new PageUri(WallhavenClient.GetSearchUri("010", "010")),
                new ContentCategory(Category.Girls_Fantasy, 30, 39)),

            CreateCrawlOffer(
                "Anime_NSFW",
                new PageUri(WallhavenClient.GetSearchUri("010", "001")),
                new ContentCategory(Category.Girls_Fantasy, 40, 69)),

            CreateCrawlOffer(
                "People_SFW",
                new PageUri(WallhavenClient.GetSearchUri("001", "100")),
                new ContentCategory(Category.Girls, 10, 29)),

            CreateCrawlOffer(
                "People_Sketchy",
                new PageUri(WallhavenClient.GetSearchUri("001", "010")),
                new ContentCategory(Category.Girls, 30, 39)),

            CreateCrawlOffer(
                "People_NSFW",
                new PageUri(WallhavenClient.GetSearchUri("001", "001")),
                new ContentCategory(Category.Girls, 40, 69))
        ];
        result.ForEach(x => x.CrawlMethod = CrawlMethod.Api);
        return Task.FromResult(result);
    }




    //z.B. "https://wallhaven.cc/api/v1/search?categories=001&purity=100&sorting=date_added&order=desc&page=1"
    protected override PageUri GetSiteUrlForCategory(CrawlOffer catJob) =>
        catJob.CategoryUri.WithParam("page", catJob.CurrentPage.ToString());

    //the api delivers json, the html path of the base class is not used
    protected override string GetSearchStringGorEntryNodes() => string.Empty;
    protected override Task<bool> AddWallEntry(PageNode pageNode, CrawlOffer catJob) => Task.FromResult(false);

    protected override async Task<bool> HandlePage(CrawlOffer catJob)
    {
        var pageUri = GetSiteUrlForCategory(catJob);
        WallhavenSearchResult search;
        try
        {
            search = await _client.Search(pageUri.Uri, CancellationToken);
        }
        catch (HttpRequestException ex)
        {
            AddWarning(pageUri, $"Search failed: {ex.Message}");
            return false;
        }

        //no entries on page
        if (search.Data.Length == 0)
            return false;

        catJob.ReportNumberOfEntriesPerPage(search.Meta.PerPage);
        foreach (var item in search.Data)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (catJob.ReachedMaximumStreak)
            {
                catJob.ReportEndReached();
                return true;
            }
            await AddApiEntry(item, catJob);
        }
        catJob.ReportPageDone();

        if (search.Meta.CurrentPage >= search.Meta.LastPage)
            catJob.ReportEndReached();
        return true;
    }

    private async Task AddApiEntry(WallhavenListItem item, CrawlOffer catJob)
    {
        var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
        {
            ImageUri = new PageUri(new Uri(item.Path)),
            ThumbnailUri = new PageUri(new Uri(item.Thumbs.Original))
        };

        //tags cost a detail call each, only new walls need them
        if (!IsKnown(item.Path))
        {
            try
            {
                var wallpaper = await _client.GetWallpaper(item.Id, CancellationToken);
                source.Tags.AddRange(wallpaper.Tags.Select(x => x.ToTag()));
            }
            catch (HttpRequestException ex)
            {
                //skipped, so it stays new and comes again with the next crawl
                AddWarning(source.ImageUri, $"Details of {item.Id} failed: {ex.Message}");
                return;
            }
        }

        if (source.ToWallEntry() is not { } entry)
            return;

        AddEntry(entry, catJob);
    }

}
