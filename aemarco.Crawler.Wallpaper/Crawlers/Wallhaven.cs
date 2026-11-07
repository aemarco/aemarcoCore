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
        ILogger<Wallhaven> logger)
        : base(logger)
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
        return Task.FromResult(result);
    }


    protected override async Task<bool> HandlePage(CrawlOffer catJob)
    {
        //z.B. "https://wallhaven.cc/api/v1/search?categories=001&purity=100&sorting=date_added&order=desc&page=1"
        var pageUri = catJob.CategoryUri.WithParam("page", catJob.CurrentPage.ToString());
        //a failed search is not the end of the pages, it fails the offer (see RunOffer): the entries of the
        //pages before would be known at the next crawl, which then ends before the page which was missed
        var search = await _client.Search(pageUri.Uri, CancellationToken);

        //no entries on page
        if (search.Data.Length == 0)
            return false;

        Logger.LogDebug("Wallhaven offer {offer} page {page} delivers {count} walls", catJob.SiteCategoryName, catJob.CurrentPage, search.Data.Length);
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
        if (!IsKnown(item.Path, catJob))
        {
            try
            {
                var wallpaper = await _client.GetWallpaper(item.Id, CancellationToken);
                source.Tags.AddRange(wallpaper.Tags.Select(x => x.ToTag()));
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                //gone for good, so it is skipped (it asks again with every crawl, but never blocks the offer);
                //any other failure is transient and fails the offer instead, a skipped wall would be
                //left behind once ten known walls precede it
                AddWarning(source.ImageUri!, $"Details of {item.Id} not found: {ex.Message}");
                return;
            }
        }

        if (source.ToWallEntry() is not { } entry)
            return;

        AddEntry(entry, catJob);
    }

}
