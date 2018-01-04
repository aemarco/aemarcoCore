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
    public class WallpaperCrawlerFtop : BildCrawlerBasis
    {
        const string _url = "https://ftopx.com/";
        const string _siteName = "ftopx";


        public WallpaperCrawlerFtop(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerFtop(
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

                result.Add(url, text);
            }

            return result;
        }

        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
            return $"{categoryUrl}page/{page}/?sort=p.approvedAt&direction=desc";
        }

        protected override string GetSearchStringGorEntry()
        {
            return "//div[@class='thumbnail']/a";
        }

        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Celebrities":
                    return new ContentCategory(Category.Girls, Category.Celebrities);
                case "Girls & Beaches":
                    return new ContentCategory(Category.Girls, Category.Beaches);
                case "Girls & Cars":
                    return new ContentCategory(Category.Girls, Category.Cars);
                case "Girls & Bikes":
                    return new ContentCategory(Category.Girls, Category.Bikes);
                case "Lingerie Girls":
                    return new ContentCategory(Category.Girls, Category.Lingerie);
                case "Asian Girls":
                    return new ContentCategory(Category.Girls, Category.Asian);
                case "Holidays":
                    return new ContentCategory(Category.Girls, Category.Holidays);
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    return new ContentCategory(Category.Girls, Category.Fantasy);
                case "Celebrity Fakes":
                    return new ContentCategory(Category.Girls, Category.CelebrityFakes);
                case "Fetish Girls":
                    return new ContentCategory(Category.Girls, Category.Fetish);
                default:
                    return new ContentCategory(Category.Girls, Category.None);
            }
        }

        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            //z.B. "https://ftopx.com/images/201712/ftopx.com_5a4482e5acc2d.jpg"
            string url = GetImageUrl(node.Attributes["href"]?.Value);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                ContentCategory = GetContentCategory(categoryName),
                Tags = GetTagsFromTagString(node.SelectSingleNode("./img")?.Attributes["alt"]?.Value),
                Url = url,
                ThumbnailUrl = GetThumbnailUrlAbsolute(node),
                FileName = GetFileName(url, $"{categoryName}_"),
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

        private string GetImageUrl(string href)
        {
            if (String.IsNullOrEmpty(href))
            {
                return null;
            }

            //z.B. "211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string id = href.Substring(href.LastIndexOf("/") + 1);
            //z.B. "211314"
            id = id.Substring(0, id.IndexOf('-'));

            //z.B. "https://ftopx.com/211314/0_0"
            string wallPage = $"{_url}{id}/0_0";

            HtmlDocument doc = GetDocument(wallPage);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@type='button']");
            if (node == null)
            {
                return null;
            }
            return node.Attributes["href"]?.Value;
        }


    }
}
