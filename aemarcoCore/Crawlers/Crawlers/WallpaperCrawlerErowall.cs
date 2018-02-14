﻿using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerErowall : WallpaperCrawlerBasis
    {
        const string _url = "https://erowall.com/";

        public WallpaperCrawlerErowall(
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

            //main page
            var doc = GetDocument(_url);

            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='m']/li[@class='m']/a"))
            {

                //z.B. "#brunette"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || !text.StartsWith("#"))
                {
                    continue;
                }
                else
                {
                    //z.B. "brunette"
                    text = text.Substring(1);
                    if (String.IsNullOrEmpty(text))
                    {
                        continue;
                    }
                    //z.B. "Brunette"
                    text = char.ToUpper(text[0]) + text.Substring(1);
                }



                //z.B. "/search/brunette/"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "search/brunette/"
                href = href.Substring(1).Replace("search", "teg");


                //z.B. "https://erowall.com/search/brunette/"
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
            //z.B. "https://erowall.com/teg/brunette/page/1"       
            return $"{categoryUrl}page/{page}";
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='wpmini']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Blowjob":
                    return new ContentCategory(Category.Girls_Blowjob);
                case "Lesbians":
                    return new ContentCategory(Category.Girls_Lesbians);
                case "Lingerie":
                    return new ContentCategory(Category.Girls_Lingerie);
                case "Beach":
                    return new ContentCategory(Category.Girls_Beaches);
                case "Asian":
                    return new ContentCategory(Category.Girls_Asian);
                case "Anime":
                    return new ContentCategory(Category.Girls_Fantasy);
                default:
                    return new ContentCategory(Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            //details
            Match match = Regex.Match(node.Attributes["href"]?.Value, @"/(\d+)/$");
            // z.B. "24741"
            string imageLink = match.Groups[1].Value;
            // z.B. "https://erowall.com//wallpapers/original/24741.jpg"
            string url = _url + "wallpapers/original/" + imageLink + ".jpg";



            var source = new WallEntrySource(new Uri(_url), node, categoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node);

            //details
            source.ImageUri = new Uri(_url + "wallpapers/original/" + imageLink + ".jpg");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='view-left']/a/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, categoryName);
            source.ContentCategory = GetContentCategory(categoryName);
            source.Tags = source.GetTagsFromNode(node, "title");


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
