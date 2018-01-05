using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using aemarcoCore.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerAdultWalls : WallpaperCrawlerBasis
    {
        const string _url = "http://adultwalls.com/";
        const string _siteName = "adultwalls";


        public WallpaperCrawlerAdultWalls(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerAdultWalls(
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

            //main page
            var doc = GetDocument(_url);

            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//li[@class='sub-menu-item']/a"))
            {

                //z.B. "Erotic Wallpapers"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text))
                {
                    continue;
                }

                //z.B. "/wallpapers/erotic-wallpapers"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "wallpapers/erotic-wallpapers"
                href = href.Substring(1);
                //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers"
                string url = $"{_url}{href}/";

                result.Add(url, text);
            }

            return result;
        }

        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers/1?order=publish-date-newest&resolution=all&search="                
            return $"{categoryUrl}{page}?order=publish-date-newest&resolution=all&search=";
        }

        protected override string GetSearchStringGorEntry()
        {
            return "//div[@class='thumb-container']/a";
        }

        protected override string GetFileName(string url, string prefix)
        {
            ////z.B. "http://adultwalls.com/web/wallpapers/closeup-sexy-ass-black-bikini-model/1920x1080.jpg"
            string name = url.Substring(url.IndexOf("wallpapers/") + 11);
            name = name.Substring(0, name.IndexOf("/"));
            return name;
        }

        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Lingerie Models":
                    return new ContentCategory(Category.Girls, Category.Lingerie);
                default:
                    return new ContentCategory(Category.Girls, Category.None);
            }
        }

        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            // z.B. "wallpaper/shot-jeans-topless-brunette-model"
            string detailsHref = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(detailsHref))
            {
                return false;
            }
            HtmlDocument detailsDoc = GetDocument($"{_url}{detailsHref}");

            //z.B. "http://adultwalls.com/web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string url = GetImageUrl(detailsDoc);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                ContentCategory = GetContentCategory(categoryName),
                Tags = GetTagsFromNodes(detailsDoc.DocumentNode.SelectNodes("//div[@class='col-md-12']/a")),
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
            HtmlNode node = doc?.DocumentNode.SelectSingleNode("//a[@class='btn btn-danger']");
            if (node == null)
            {
                return null;
            }

            // z.B. "wallpaper/shot-jeans-topless-brunette-model/1920x1080"
            string href = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(href))
            {
                return null;
            }

            //z.B. "http://adultwalls.com/wallpaper/shot-jeans-topless-brunette-model/1920x1080"
            string page = $"{_url}{href}";


            HtmlDocument doc2 = GetDocument(page);
            HtmlNode urlnode = doc2.DocumentNode.SelectSingleNode("//div[@class='wallpaper-preview-container']/a/img");
            if (urlnode == null)
            {
                return null;
            }

            // z.B. "web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string urlHref = urlnode.Attributes["src"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(urlHref))
            {
                return null;
            }

            // z.B. "http://adultwalls.com/web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string url = _url + urlHref;

            return url;
        }





    }
}
