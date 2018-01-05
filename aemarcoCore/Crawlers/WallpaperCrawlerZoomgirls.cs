using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using aemarcoCore.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerZoomgirls : WallpaperCrawlerBasis
    {
        const string _url = "https://zoomgirls.net/";
        const string _siteName = "zoomgirls";


        public WallpaperCrawlerZoomgirls(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerZoomgirls(
            int startPage,
            int lastPage,
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, startPage, lastPage, progress, cancellationToken)
        {

        }


        protected override Dictionary<string, string> GetCategoriesDict()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            //z.B. "Latest Wallpapers"
            string text = "Latest Wallpapers";
            //z.B. "latest_wallpapers"
            string href = "latest_wallpapers";
            //z.B. "https://zoomgirls.net/latest_wallpapers"
            string url = $"{_url}{href}";
            result.Add(url, text);

            return result;
        }

        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
            return $"{categoryUrl}/page/{page}";
        }

        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb']/a";
        }

        protected override IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.Girls, Category.None);
        }

        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            string docURL = _url + node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(docURL))
            {
                return false;
            }
            HtmlDocument doc = GetDocument(docURL);

            //z.B. "https://zoomgirls.net/wallpapers/amy-addison--1920x1200.jpg"
            string url = GetImageUrl(doc);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                ContentCategory = GetContentCategory(categoryName),
                Tags = GetTagsFromNodes(doc.DocumentNode.SelectNodes("//ul[@class='tagcloud']/span/a")),
                Url = url,
                ThumbnailUrl = GetThumbnailUrlRelative(_url, node),
                FileName = GetFileName(url, string.Empty),
                Extension = FileExtension.GetFileExtension(url)
            };
            //Entry muss valid sein
            if (!wallEntry.IsValid())
            {
                return false;
            }

            AddEntry(wallEntry);
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
            int maxSum = 0;
            foreach (var node in allNodes)
            {
                //get both number values
                string[] txt = node.Attributes["title"]?.Value?.Split('x');
                if (txt != null && txt.Length == 2)
                {
                    int sum = 0;

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


            //z.B. "/view-jana-jordan--1920x1200.html"
            string url = targetNode.Attributes["href"]?.Value;
            //z.B. "jana-jordan--1920x1200.html"
            url = url.Substring(url.IndexOf("view") + 5);
            //z.B. "jana-jordan--1920x1200"
            url = url.Substring(0, url.IndexOf(".html"));
            //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"
            url = _url + @"wallpapers/" + url + ".jpg";


            return url;

        }



    }
}
