using aemarcoCore.Common;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class WallpaperCrawlerAdultWallsTests
    {






        [Test]
        public void Crawler_Finds_GirlsLingerie_Offer()
        {
            var crawler = new WallpaperCrawlerAdultWalls(1, 10, CancellationToken.None, true);
            crawler.LimitAsPerFilterlist(new List<string> { Category.Girls_Lingerie.ToString() });
            Assert.IsTrue(crawler.HasWorkingOffers);
        }

        [Test]
        public void Crawler_Finds_Girls_Offer()
        {
            var crawler = new WallpaperCrawlerAdultWalls(1, 10, CancellationToken.None, true);
            crawler.LimitAsPerFilterlist(new List<string> { Category.Girls.ToString() });
            Assert.IsTrue(crawler.HasWorkingOffers);
        }









    }
}
