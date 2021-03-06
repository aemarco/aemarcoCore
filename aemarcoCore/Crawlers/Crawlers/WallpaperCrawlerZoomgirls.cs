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

    internal class WallpaperCrawlerZoomgirls : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://zoomgirls.net");

        internal override SourceSite SourceSite => SourceSite.Zoomgirls;

        public WallpaperCrawlerZoomgirls(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>();

            
            result.Add(CreateCrawlOffer(
                "Latest Wallpapers", 
                new Uri(_uri, "latest_wallpapers"), 
                DefaultCategory));
            result.Add(CreateCrawlOffer(
                "Random Wallpapers", 
                new Uri(_uri, "random_wallpapers"), 
                DefaultCategory));
            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
            //return $"{catJob.CategoryUri.AbsoluteUri}/page/{catJob.CurrentPage}";
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/page/{ catJob.CurrentPage }");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb']/a";
        }

        protected override IContentCategory DefaultCategory => new ContentCategory(Category.Girls);
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();

            //details
            var imageUri = GetImageUrl(source.DetailsDoc);
            if (string.IsNullOrEmpty(imageUri)) return false;

            source.ImageUri = new Uri(imageUri);
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='wallpaper-thumb']/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
            source.ContentCategory = catJob.Category;
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@class='tagcloud']/span/a", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));

            var wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }
        private string GetImageUrl(HtmlDocument doc)
        {
            HtmlNode targetNode = null;


            //select all resolution nodes
            var allNodes = doc.DocumentNode.SelectNodes("//div[@class='tagcloud']/span/a");
            if (allNodes == null)
            {
                return null;
            }
            //search for node with highest resolution
            var maxSum = 0;
            foreach (var node in allNodes)
            {
                //get both number values
                var txt = node.Attributes["title"]?.Value?.Split('x');
                if (txt != null && txt.Length == 2)
                {
                    var sum = 0;

                    try
                    {
                        //do the math
                        sum = int.Parse(txt[0].Trim()) * int.Parse(txt[1].Trim());
                    }
                    catch
                    {
                        continue;
                    }

                    //set to targetNode if sum is highest
                    if (sum > maxSum)
                    {
                        maxSum = sum;
                        targetNode = node;
                    }
                }
            }

            if (targetNode == null)
            {
                return null;
            }



            #region Image Url

            //z.B. "/view-jana-jordan--1920x1200.html"
            var name = targetNode.Attributes["href"]?.Value;
            //z.B. "jana-jordan--1920x1200.html"
            name = name.Substring(name.IndexOf("view") + 5);
            //z.B. "jana-jordan--1920x1200"
            name = name.Substring(0, name.IndexOf(".html"));
            //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"

            var url = new Uri(_uri, $"/wallpapers/{name}.jpg").AbsoluteUri;

            #endregion

            return url;

        }


    }
}
