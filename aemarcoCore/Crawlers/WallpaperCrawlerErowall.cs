using aemarcoCore.Common;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;



namespace aemarcoCore.Crawlers
{
    internal class WallpaperCrawlerErowall : WallpaperCrawlerBasis
    {
        const string _url = "https://erowall.com/";


        internal WallpaperCrawlerErowall(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(startPage, lastPage, cancellationToken)
        {

        }


        protected override Dictionary<string, string> GetCategoriesDict()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

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

                result.Add(url, text);
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
                    return new ContentCategory(Category.Girls, Category.Blowjob);
                case "Lesbians":
                    return new ContentCategory(Category.Girls, Category.Lesbians);
                case "Lingerie":
                    return new ContentCategory(Category.Girls, Category.Lingerie);
                case "Beach":
                    return new ContentCategory(Category.Girls, Category.Beaches);
                case "Asian":
                    return new ContentCategory(Category.Girls, Category.Asian);
                case "Anime":
                    return new ContentCategory(Category.Girls, Category.Fantasy);
                default:
                    return new ContentCategory(Category.Girls, Category.None);
            }
        }

        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            // z.B. "https://erowall.com//wallpapers/original/24741.jpg"
            string url = GetImageUrl(node.Attributes["href"]?.Value);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
                (
                url,
                GetThumbnailUrlRelative(_url, node),
                GetFileName(url, $"{categoryName}_"),
                GetContentCategory(categoryName),
                categoryName,
                GetTagsFromTagString(node.Attributes["title"]?.Value)
                );

            //Entry muss valid sein
            if (!wallEntry.IsValid)
            {
                return false;
            }

            AddEntry(wallEntry);
            return true;
        }

        private string GetImageUrl(string href)
        {
            if (href == null)
            {
                return null;
            }

            Match match = Regex.Match(href, @"/(\d+)/$");
            // z.B. "24741"
            string imageLink = match.Groups[1].Value;
            // z.B. "https://erowall.com//wallpapers/original/24741.jpg"
            string url = _url + "/wallpapers/original/" + imageLink + ".jpg";

            return url;
        }


    }
}
