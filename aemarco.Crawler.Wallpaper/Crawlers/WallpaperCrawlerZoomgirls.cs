using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;

namespace aemarco.Crawler.Wallpaper.Crawlers
{
    [WallpaperCrawler("Zoomgirls")]
    internal class WallpaperCrawlerZoomgirls : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://zoomgirls.net");



        public WallpaperCrawlerZoomgirls(
            int startPage,
            int lastPage,
            bool onlyNews)
            : base(startPage, lastPage, onlyNews)
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
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/page/{catJob.CurrentPage}");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb']/a";
        }

        protected override ContentCategory DefaultCategory => new ContentCategory(Category.Girls);
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
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@class='tagcloud']/span/a", x => WebUtility.HtmlDecode(x.InnerText).Trim());

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
                    int sum;

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

            if (name is null)
                return null;


            //z.B. "jana-jordan--1920x1200.html"
            name = name[(name.IndexOf("view", StringComparison.Ordinal) + 5)..];
            //z.B. "jana-jordan--1920x1200"
            name = name[..name.IndexOf(".html", StringComparison.Ordinal)];
            //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"

            var url = new Uri(_uri, $"/wallpapers/{name}.jpg").AbsoluteUri;

            #endregion

            return url;

        }


    }
}
