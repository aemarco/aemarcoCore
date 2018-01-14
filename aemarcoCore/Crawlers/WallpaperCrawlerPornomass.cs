using aemarcoCore.Common;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    internal class WallpaperCrawlerPornomass : WallpaperCrawlerBasis
    {
        const string _url = "http://pornomass.com/";
        const string _url2 = "http://gif.pornomass.com/";


        internal WallpaperCrawlerPornomass(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(startPage, lastPage, cancellationToken)
        {

        }


        protected override Dictionary<string, string> GetCategoriesDict()
        {
            Dictionary<string, string> result = new Dictionary<string, string>
            {
                { _url, "Pornomass" },
                { _url2, "Gifpornomass" }
            };

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

        protected override string GetThumbnailUrlRelative(string url, HtmlNode node)
        {
            HtmlNode imageNode;
            string attribute;
            if (url == _url)
            {
                imageNode = node.SelectSingleNode("./img");
                attribute = "src";
            }
            else if (url == _url2)
            {
                imageNode = node.SelectSingleNode("./video");
                attribute = "poster";
            }
            else { return string.Empty; }

            return $"{url}{imageNode?.Attributes[attribute]?.Value?.Substring(1)}";
        }

        protected override IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.Girls, Category.Hardcore);
        }




        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            string site = string.Empty;
            string siteName = string.Empty;
            string thumbnail = string.Empty;
            if (categoryName == "Pornomass")
            {
                site = _url;
                thumbnail = GetThumbnailUrlRelative(_url, node);
            }
            else
            {
                site = _url2;
                thumbnail = GetThumbnailUrlRelative(_url2, node);
            }

            // z.B. "http://pornomass.com/uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            // z.B. "http://gif.pornomass.com/uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            string url = GetImageUrl(site, node.Attributes["href"]?.Value?.Substring(1));
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
                (
                url,
                thumbnail,
                GetFileName(url, string.Empty),
                GetContentCategory(categoryName),
                categoryName,
                new List<string>()
                );

            //Entry muss valid sein
            if (!wallEntry.IsValid)
            {
                return false;
            }

            AddEntry(wallEntry);
            return true;
        }

        private string GetImageUrl(string site, string href)
        {
            if (href == null)
            {
                return null;
            }

            //z.B. "http://pornomass.com/photo/1949-xxx.html" -- "Pornomass"
            //z.B. "http://gif.pornomass.com/photo/614-beautiful-girl-anal-gif.html" -- "Gifpornomass"
            string targetUrl = $"{site}{href}";

            HtmlDocument doc = GetDocument(targetUrl);
            HtmlNode targetNode = doc.DocumentNode.SelectSingleNode("//a[@class='photo-blink']");
            if (targetNode == null)
            {
                return null;
            }

            //z.B. "uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            //z.B. "uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            string targetHref = targetNode.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(targetHref))
            {
                return null;
            }

            //z.B. "http://pornomass.com/uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            //z.B. "http://gif.pornomass.com/uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            return $"{site}{targetHref}";
        }





    }
}
