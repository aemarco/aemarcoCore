using aemarco.Crawler.Wallpaper;
using aemarco.Crawler.Wallpaper.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.WallpaperTests
{
    [SingleThreaded]
    public class WallpaperCrawlerTests
    {
        private IConfiguration _config;

        private List<string> _sourceSites;
        private List<string> _sourceCategories;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables("APIKEY:")
                .Build();


            var crawler = GetCrawler();
            _sourceSites = crawler.GetAvailableSourceSites().ToList();
            _sourceCategories = crawler.GetAvailableCategories().ToList();
        }

        [Test]
        public void GetAvailableCategories_Delivers()
        {
            _sourceCategories.Count.Should().BeGreaterThan(0);
        }


        [Test]
        public void HandleFilter_HandlesSites()
        {
            //filter does filter
            foreach (var site in _sourceSites)
            {
                var c = GetCrawler();
                c.AddSourceSiteFilter(site);
                c.HandleFilters();
                c._wallCrawlers.Select(x => x.GetType().ToCrawlerInfo().FriendlyName).Should().BeEquivalentTo(site);
            }

            //no filter does not filter
            var all = GetCrawler();
            all.HandleFilters();
            all._wallCrawlers.Select(x => x.GetType().ToCrawlerInfo().FriendlyName).Should()
                .BeEquivalentTo(_sourceSites);
        }

        [Test]
        public void HandleFilter_HandlesCategories()
        {
            //filter does filter
            var rand = _sourceCategories.FirstOrDefault();
            rand.Should().NotBeNullOrWhiteSpace();
            var c = GetCrawler();
            c.AddCategoryFilter(rand);
            c.HandleFilters();
            Assert.IsTrue(c._wallCrawlers.All(x => x._crawlOffers.All(o => o.Category.Category == rand)));

            //no filter does not filter
            var all = GetCrawler();
            all.HandleFilters();
            Assert.IsTrue(all._wallCrawlers.All(x => x._crawlOffers.Any()));

        }

        [Test]
        public async Task Start_DoesWork()
        {


            foreach (var site in _sourceSites)
            {
                foreach (var cat in _sourceCategories)
                {
                    var crawler = GetCrawler();
                    crawler.AddSourceSiteFilter(site);
                    crawler.AddCategoryFilter(cat);

                    crawler.HandleFilters();

                    if (crawler._wallCrawlers.Any(c => c._crawlOffers.Any(o => o.Category.Category == cat)))
                    {
                        var source = new CancellationTokenSource();

                        var c = crawler._wallCrawlers.First();
                        c.EntryFound += (sender, e) =>
                        {
                            source.Cancel();
                        };
                        try
                        {
                            await crawler.StartAsync(source.Token);
                            Assert.Fail();
                        }
                        catch (OperationCanceledException)
                        { }
                    }
                }
            }
        }

        [Test]
        public async Task TestSingleCrawl()
        {
            var site = "Mota";
            var cat = "Girls";


            var crawler = GetCrawler();
            crawler.AddSourceSiteFilter(site);
            crawler.AddCategoryFilter(cat);

            crawler.HandleFilters();

            if (crawler._wallCrawlers.Any(c => c._crawlOffers.Any(o => o.Category.Category == cat)))
            {
                var source = new CancellationTokenSource();

                var c = crawler._wallCrawlers.First();
                c.EntryFound += (sender, e) =>
                {
                    source.Cancel();
                };
                try
                {
                    await crawler.StartAsync(source.Token);
                    Assert.Fail();
                }
                catch (OperationCanceledException)
                { }
            }

        }






        private WallpaperCrawler GetCrawler()
        {
            var result = new WallpaperCrawler(1, 1);
            var apiKey = _config.GetValue<string>("AbyssKey");
            result.Configure(abyssApiKey: apiKey);
            return result;
        }
    }
}
