using aemarcoCore.Common;
using aemarcoCore.Crawlers;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    [SingleThreaded]
    public class WallpaperCrawlerTests
    {
        [Test]
        public void SourceSites_HaveACrawler()
        {
            foreach (SourceSite site in Enum.GetValues(typeof(SourceSite)))
            {
                var crawler = new WallpaperCrawler(CancellationToken.None, null, 0, 0);
                crawler.AddSourceSiteFilter(site);
                crawler.PrepareCrawlerList();

                var worker = crawler._crawlers.Keys.FirstOrDefault();
                Assert.IsTrue(worker != null && worker.SourceSite == site);
            }
        }


        static object[] CategoryCases
        {
            get
            {
                return Enum.GetValues(typeof(Category))
                    .Cast<Category>()
                    .Select(x => new object[] { x })
                    .ToArray();
            }
        }
        [TestCaseSource("CategoryCases")]
        public void Crawlers_Keep_Support_Promises(Category cat)
        {
            var sites = Enum.GetValues(typeof(SourceSite))
                    .Cast<SourceSite>()
                    .Where(x => x != SourceSite.Abyss) //Abyss no offers because Api-Key
                    .ToList();

            var sitesSupporting = sites.Where(x => x.Supports(cat.ToString())).ToList();
            var crawler = new WallpaperCrawler(CancellationToken.None, null, 0, 0);
            crawler.AddCategoryFilter(cat);
            crawler.PrepareCrawlerList();

            //crawlers only added to list if they have offers
            //check HasWorkingOffers anyways                       
            //crawlers need to truely support a category, because 
            //GetCrawlsOffers will visit the site
            //LimitAsPerFilterlist will filter list to desired category

            Assert.IsTrue(crawler._crawlers.Keys.Count == sitesSupporting.Count);
            Assert.IsTrue(crawler._crawlers.Keys.All(c => c.HasWorkingOffers));
            sitesSupporting.ForEach(s =>
            {
                //each supported site must have exactly 1 crawler
                Assert.IsTrue(crawler._crawlers.Keys.Single(k => k.SourceSite == s) != null);
            });

        }



        static object[] CrawlCases
        {
            get
            {
                List<object> result = new List<object>();

                foreach (SourceSite site in Enum.GetValues(typeof(SourceSite)))
                {
                    if (site == SourceSite.Abyss) continue; //Abyss no Api Key
                    foreach (Category cat in Enum.GetValues(typeof(Category)))
                    {
                        if (site.Supports(cat.ToString()))
                        {
                            result.Add(new object[] { site, cat });
                        }
                    }
                }
                return result
                    .ToArray();
            }
        }
        [TestCaseSource("CrawlCases")]
        public void Crawlers_Finds_Entries(SourceSite site, Category cat)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var crawler = new WallpaperCrawler(cts.Token, null, 1, 1);
            crawler.AddSourceSiteFilter(site);
            crawler.AddCategoryFilter(cat);

            bool found = false;
            crawler.NewEntry += (sender, e) =>
            {
                found = true;
                if (!cts.IsCancellationRequested) cts.Cancel();
            };
            var result = crawler.StartAsyncTask().GetAwaiter().GetResult();

            Assert.IsTrue(found || result.NewEntries.Count > 0);

            Task.Delay(5000).GetAwaiter().GetResult();
        }






    }
}
