﻿using aemarcoCore;
using aemarcoCore.Common;
using aemarcoCore.Crawlers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    [SingleThreaded]
    public class WallpaperCrawlerTests
    {
        private IConfiguration _config;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables("APIKEY:")
                .Build();
        }

        [SetUp]
        public void Setup()
        {
            ConfigurationHelper.KnownUrlsFunc = () => new List<string>();
            ConfigurationHelper.AbyssApiKey = _config.GetValue<string>("AbyssKey");
        }

        [TearDown]
        public void TearDown()
        {
            var app = AppDomain.CurrentDomain.BaseDirectory;
            var di = new DirectoryInfo(Path.Combine(app!, "temp"));

            if (di.Exists) di.Delete(true);
        }


        public static object[] SiteCases
        {
            get
            {
                // ReSharper disable once CoVariantArrayConversion
                return Enum.GetValues(typeof(SourceSite))
                    .Cast<SourceSite>()
                    .Where(x => !x.IsDisabled())
                    .Select(x => new object[] { x })
                    .ToArray();
            }
        }
        [TestCaseSource(nameof(SiteCases))]
        public void SourceSites_HaveACrawler(SourceSite site)
        {
            if (site == SourceSite.Abyss && string.IsNullOrWhiteSpace(ConfigurationHelper.AbyssApiKey))
                Assert.Pass(); // skip abyss when there is no API key


            var crawler = new WallpaperCrawler(CancellationToken.None);
            crawler.AddSourceSiteFilter(site);
            crawler.PrepareCrawlerList();

            var worker = crawler._crawlers.Keys.FirstOrDefault();

            worker.Should().NotBeNull();
            worker!.SourceSite.Should().Be(site);

        }


        public static object[] DisabledSites
        {
            get
            {
                // ReSharper disable once CoVariantArrayConversion
                return Enum.GetValues(typeof(SourceSite))
                    .Cast<SourceSite>()
                    .Where(x => x.IsDisabled())
                    .Select(x => new object[] { x })
                    .ToArray();
            }
        }
        [TestCaseSource(nameof(DisabledSites))]
        public void Crawlers_Disabled_DontCrawl(SourceSite site)
        {
            var crawler = new WallpaperCrawler(CancellationToken.None);
            crawler.AddSourceSiteFilter(site);
            crawler.PrepareCrawlerList();

            var worker = crawler._crawlers.Keys.FirstOrDefault();

            worker.Should().BeNull();

        }


        public static object[] CategoryCases
        {
            get
            {
                // ReSharper disable once CoVariantArrayConversion
                return Enum.GetValues(typeof(Category))
                    .Cast<Category>()
                    .Select(x => new object[] { x })
                    .ToArray();
            }
        }
        [TestCaseSource(nameof(CategoryCases))]
        public void Crawlers_Keep_Support_Promises(Category cat)
        {
            Task.Delay(1000).GetAwaiter().GetResult();


            var sites = Enum.GetValues(typeof(SourceSite))
                    .Cast<SourceSite>()
                    .Where(x => !x.IsDisabled())
                    .Where(x => x != SourceSite.Abyss || !string.IsNullOrWhiteSpace(ConfigurationHelper.AbyssApiKey)) // skip abyss when there is no API key
                    .ToList();

            var sitesSupporting = sites.Where(x => x.Supports(cat.ToString())).ToList();
            var crawler = new WallpaperCrawler(CancellationToken.None);
            crawler.AddCategoryFilter(cat);
            crawler.PrepareCrawlerList();

            //crawlers only added to list if they have offers
            //check HasWorkingOffers anyways                       
            //crawlers need to truely support a category, because 
            //GetCrawlsOffers will visit the site
            //LimitAsPerFilterlist will filter list to desired category


            crawler._crawlers.Keys.Count.Should().Be(sitesSupporting.Count);
            Assert.IsTrue(crawler._crawlers.Keys.All(c => c.HasWorkingOffers));
            sitesSupporting.ForEach(s =>
            {
                //each supported site must have exactly 1 crawler for given category
                crawler._crawlers.Keys.Single(k => k.SourceSite == s).Should().NotBeNull();
            });

        }

        private static readonly Random _random = new Random();
        public static object[] CrawlCases
        {
            get
            {
                var result = new List<object>();
                foreach (SourceSite site in Enum.GetValues(typeof(SourceSite)))
                {
                    if (site.IsDisabled()) continue;
                    foreach (Category cat in Enum.GetValues(typeof(Category)))
                    {
                        if (site.Supports(cat.ToString()))
                        {
                            result.Add(new object[] { site, cat });
                        }
                    }
                }
                return result.OrderBy(x => _random.Next()).ToArray();
            }
        }
        [TestCaseSource(nameof(CrawlCases))]
        public void Crawlers_Finds_Entries(SourceSite site, Category cat)
        {
            if (site == SourceSite.Abyss && string.IsNullOrWhiteSpace(ConfigurationHelper.AbyssApiKey))
                Assert.Pass(); // skip abyss when there is no API key

            using var cts = new CancellationTokenSource();
            var crawler = new WallpaperCrawler(cts.Token, null, 1, 1);
            crawler.AddSourceSiteFilter(site);
            crawler.AddCategoryFilter(cat);

            
            // ReSharper disable AccessToDisposedClosure
            crawler.NewEntry += (sender, e) =>
            {
                
                if (!cts.IsCancellationRequested) cts.Cancel();
            };
            crawler.NewAlbum += (sender, e) =>
            {
                
                if (!cts.IsCancellationRequested) cts.Cancel();
            };
            // ReSharper restore AccessToDisposedClosure


            var result = crawler.StartAsyncTask().GetAwaiter().GetResult();
            Assert.IsTrue(result.HasBeenAborted);
            cts.Dispose();
        }



    }
}
