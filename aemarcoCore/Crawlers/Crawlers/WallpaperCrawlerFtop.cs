﻿using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerFtop : WallpaperCrawlerBasis
    {
        const string _url = "https://ftopx.com/";

        public WallpaperCrawlerFtop(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(startPage, lastPage, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            //main page
            var doc = GetDocument(_url);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            {

                //z.B. "Celebrities"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "Sandbox")
                {
                    continue;
                }

                //z.B. "/celebrities/
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "celebrities/"
                href = href.Substring(1);
                //z.B. "https://ftopx.com/celebrities"
                string url = $"{_url}{href}";

                IContentCategory cat = GetContentCategory(text);

                CrawlOffer offer = new CrawlOffer
                {
                    Name = text,
                    Url = url,
                    MainCategory = cat.MainCategory,
                    SubCategory = cat.SubCategory
                };

                result.Add(offer);
            }

            return result;

        }
        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
            return $"{categoryUrl}page/{page}/?sort=p.approvedAt&direction=desc";
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumbnail']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Celebrities":
                    return new ContentCategory(Category.Girls_Celebrities);
                case "Girls & Beaches":
                    return new ContentCategory(Category.Girls_Beaches);
                case "Girls & Cars":
                    return new ContentCategory(Category.Girls_Cars);
                case "Girls & Bikes":
                    return new ContentCategory(Category.Girls_Bikes);
                case "Lingerie Girls":
                    return new ContentCategory(Category.Girls_Lingerie);
                case "Asian Girls":
                    return new ContentCategory(Category.Girls_Asian);
                case "Holidays":
                    return new ContentCategory(Category.Girls_Holidays);
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    return new ContentCategory(Category.Girls_Fantasy);
                case "Celebrity Fakes":
                    return new ContentCategory(Category.Girls_CelebrityFakes);
                case "Fetish Girls":
                    return new ContentCategory(Category.Girls_Fetish);
                default:
                    return new ContentCategory(Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            node = node.ParentNode.ParentNode;

            var source = new WallEntrySource(new Uri(_url), node, categoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node, "./div[@class='thumbnail']/a");
            source.DownloadDoc = source.GetChildDocument(source.DetailsDoc, "//div[@class='res-origin']/a");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//a[@type='button']", "href");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='img-responsive img-rounded']", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, categoryName);
            source.ContentCategory = GetContentCategory(categoryName);
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='well well-sm']/a", new Func<HtmlNode, string>(x => x.InnerText.Trim()));



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
