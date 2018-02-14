﻿using aemarcoCore.Common;
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
        private Uri _uri1 = new Uri("http://pornomass.com");
        private Uri _uri2 = new Uri("http://gif.pornomass.com");

        public WallpaperCrawlerPornomass(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
            : base(startPage, lastPage, cancellationToken, onlyNews)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            IContentCategory cat1 = GetContentCategory("Pornomass");
            result.Add(CreateCrawlOffer("Pornomass", _uri1, cat1));

            IContentCategory cat2 = GetContentCategory("Gifpornomass");
            result.Add(CreateCrawlOffer("Gifpornomass", _uri2, cat2));

            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://pornomass.com/page/1"
            //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}";
            return new Uri(catJob.CategoryUri, $"/page/{ catJob.CurrentPage }");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='fit-box']/a[@class='fit-wrapper']";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.Girls_Hardcore);
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            WallEntrySource source;

            string thumbnail = string.Empty;
            if (catJob.SiteCategoryName == "Pornomass")
            {
                source = new WallEntrySource(_uri1, node, catJob.SiteCategoryName);
                //doc
                source.DetailsDoc = source.GetDetailsDocFromNode(node);
                //details
                source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']/img", "src");

            }
            else
            {
                source = new WallEntrySource(_uri2, node, catJob.SiteCategoryName);
                //doc
                source.DetailsDoc = source.GetDetailsDocFromNode(node);
                //details
                source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']/video", "poster");

            }
            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='photo-blink']", "href");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = new List<string>();



            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }

    }
}
