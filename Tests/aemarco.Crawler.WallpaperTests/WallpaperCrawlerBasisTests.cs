using aemarco.Crawler.Common;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using Microsoft.Extensions.Logging.Abstractions;

namespace aemarco.Crawler.WallpaperTests;

//offline: a fake site with 25 pages of known entries
public class WallpaperCrawlerBasisTests
{

    [Test]
    public async Task Start_NewsNewestFirst_StopsAtKnownStreak()
    {
        var sut = new FakeCrawler(listsNewestFirst: true);

        await sut.Start(sut.KnownUrls, 1, 10, true, CancellationToken.None);

        sut.HandledPages.ShouldBe([1]);
    }

    [Test]
    public async Task Start_NewsNotNewestFirst_GoesThroughAllPages()
    {
        var sut = new FakeCrawler(listsNewestFirst: false);

        await sut.Start(sut.KnownUrls, 1, 10, true, CancellationToken.None);

        //beyond the requested 10, until the empty page 26
        sut.HandledPages.ShouldBe(Enumerable.Range(1, 26));
    }

    [Test]
    public async Task Start_PagingNotNewestFirst_KeepsPaging()
    {
        var sut = new FakeCrawler(listsNewestFirst: false);

        await sut.Start(sut.KnownUrls, 2, 4, false, CancellationToken.None);

        sut.HandledPages.ShouldBe([2, 3, 4]);
    }


    private class FakeCrawler(bool listsNewestFirst) : WallpaperCrawlerBasis(NullLogger.Instance)
    {
        private const int Pages = 25;
        private const int EntriesPerPage = 20;

        protected override bool ListsNewestFirst => listsNewestFirst;

        public HashSet<string> KnownUrls { get; } = Enumerable.Range(1, Pages)
            .SelectMany(page => Enumerable.Range(1, EntriesPerPage).Select(entry => Url(page, entry)))
            .ToHashSet();
        public List<int> HandledPages { get; } = [];

        protected override Task<List<CrawlOffer>> GetCrawlOffers() =>
            Task.FromResult<List<CrawlOffer>>(
                [CreateCrawlOffer("Fake", new PageUri(new Uri("https://fake.site/")), new ContentCategory(Category.Girls))]);

        //like HtmlCrawlerBasis: the streak is checked per entry
        protected override Task<bool> HandlePage(CrawlOffer catJob)
        {
            HandledPages.Add(catJob.CurrentPage);
            if (catJob.CurrentPage > Pages)
                return Task.FromResult(false);

            catJob.ReportNumberOfEntriesPerPage(EntriesPerPage);
            for (var entry = 1; entry <= EntriesPerPage; entry++)
            {
                if (catJob.ReachedMaximumStreak)
                {
                    catJob.ReportEndReached();
                    return Task.FromResult(true);
                }
                var source = new WallEntrySource(catJob.Category, catJob.SiteCategoryName)
                {
                    ImageUri = new PageUri(new Uri(Url(catJob.CurrentPage, entry)))
                };
                AddEntry(source.ToWallEntry()!, catJob);
            }
            catJob.ReportPageDone();
            return Task.FromResult(true);
        }

        private static string Url(int page, int entry) => $"https://fake.site/{page}/{entry}.jpg";
    }

}
