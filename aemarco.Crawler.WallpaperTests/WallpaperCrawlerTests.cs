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

        [Test]
        public void GetAvailableSites_Delivers()
        {
            Sites.Count.Should().BeGreaterThan(0);
        }
        [Test]
        public void GetAvailableCategories_Delivers()
        {
            Cats.Count.Should().BeGreaterThan(0);
        }


        [TestCaseSource(nameof(Sites))]
        public void HandleFilter_SiteFilter(string site)
        {
            //filter does filter
            var c = GetCrawler();
            c.AddSourceSiteFilter(site);
            c.HandleFilters();
            c._wallCrawlers.Select(x => x.GetType().ToCrawlerInfo().FriendlyName).Should().BeEquivalentTo(site);
        }
        [Test]
        public void HandleFilter_NoSiteFilter()
        {
            //no filter does not filter
            var all = GetCrawler();
            all.HandleFilters();
            all._wallCrawlers.Select(x => x.GetType().ToCrawlerInfo().FriendlyName).Should()
                .BeEquivalentTo(Sites);
        }


        [TestCaseSource(nameof(Cats))]
        public void HandleFilter_CatFilter(string cat)
        {
            //filter does filter
            var c = GetCrawler();
            c.AddCategoryFilter(cat);
            c.HandleFilters();
            Assert.IsTrue(c._wallCrawlers.All(x => x._crawlOffers.All(o => o.Category.Category == cat)));
        }
        [Test]
        public void HandleFilter_NoCatFilter()
        {
            //no filter does not filter
            var all = GetCrawler();
            all.HandleFilters();
            Assert.IsTrue(all._wallCrawlers.All(x => x._crawlOffers.Any()));
        }


        [TestCaseSource(nameof(CrawlerCombinations))]
        public async Task Crawler_DoesWork(string site, string cat)
        {
            var crawler = GetCrawler();
            crawler.AddSourceSiteFilter(site);
            crawler.AddCategoryFilter(cat);

            crawler.HandleFilters();

            if (crawler._wallCrawlers.Any(
                    c => c._crawlOffers.Any(
                        o => o.Category.Category == cat)))
            {
                var source = new CancellationTokenSource();

                var c = crawler._wallCrawlers.First();
                c.EntryFound += (_, _) =>
                {
                    source.Cancel();
                };
                try
                {
                    await crawler.StartAsync(source.Token);
                    Assert.Fail($"{site} - {cat} failed.");
                }
                catch (OperationCanceledException)
                { }
            }
        }


        private static List<string> Sites
        {
            get
            {
                var crawler = GetCrawler();
                var ss = crawler.GetAvailableSourceSites().ToList();
                return ss;
            }
        }
        private static List<string> Cats
        {
            get
            {
                var crawler = GetCrawler();
                var sc = crawler.GetAvailableCategories().ToList();
                return sc;
            }
        }
        private static List<object> CrawlerCombinations
        {
            get
            {
                var result = new List<object>();
                foreach (var site in Sites)
                {
                    foreach (var cat in Cats)
                    {
                        result.Add(new object[] { site, cat });
                    }
                }
                return result;
            }
        }


        private static WallpaperCrawler GetCrawler()
        {
            var result = new WallpaperCrawler(1, 1);
            var apiKey = GetConfig().GetValue<string>("AbyssKey");
            result.Configure(abyssApiKey: apiKey);
            return result;
        }
        private static IConfiguration GetConfig()
        {
            var result = new ConfigurationBuilder()
                .AddEnvironmentVariables("APIKEY:")
                .Build();
            return result;
        }

    }
}
