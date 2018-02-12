using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerPornomass : WallpaperCrawlerBasis
    {
        const string _url = "http://pornomass.com/";
        const string _url2 = "http://gif.pornomass.com/";

        public WallpaperCrawlerPornomass(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(startPage, lastPage, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            IContentCategory cat1 = GetContentCategory("Pornomass");
            result.Add(new CrawlOffer
            {
                Name = "Pornomass",
                Url = _url,
                MainCategory = cat1.MainCategory,
                SubCategory = cat1.SubCategory
            });
            IContentCategory cat2 = GetContentCategory("Gifpornomass");
            result.Add(new CrawlOffer
            {
                Name = "Gifpornomass",
                Url = _url2,
                MainCategory = cat2.MainCategory,
                SubCategory = cat2.SubCategory
            });

            return result;
        }
        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "http://pornomass.com/page/1"
            return $"{categoryUrl}page/{page}";
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='fit-box']/a[@class='fit-wrapper']";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.Girls_Hardcore);
        }
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            WallEntrySource source;

            string thumbnail = string.Empty;
            if (categoryName == "Pornomass")
            {
                source = new WallEntrySource(new Uri(_url), node, categoryName);
                //doc
                source.DetailsDoc = source.GetDetailsDocFromNode(node);
                //details
                source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']/img", "src");

            }
            else
            {
                source = new WallEntrySource(new Uri(_url2), node, categoryName);
                //doc
                source.DetailsDoc = source.GetDetailsDocFromNode(node);
                //details
                source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']/video", "poster");

            }
            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']", "href");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, categoryName);
            source.ContentCategory = GetContentCategory(categoryName);
            source.Tags = new List<string>();



            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry);
            return true;
        }

    }
}
