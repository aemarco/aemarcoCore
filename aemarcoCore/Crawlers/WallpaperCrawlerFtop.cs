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
            ContentCategory result = new ContentCategory();
            result.SetMainCategory(Category.Girls);
            switch (categoryName)
            {
                case "Celebrities":
                    {
                        result.SetSubCategory(Category.Celebrities);
                        break;
                    }
                case "Girls & Beaches":
                    {
                        result.SetSubCategory(Category.Beaches);
                        break;
                    }
                case "Girls & Cars":
                    {
                        result.SetSubCategory(Category.Cars);
                        break;
                    }
                case "Girls & Bikes":
                    {
                        result.SetSubCategory(Category.Bikes);
                        break;
                    }
                case "Lingerie Girls":
                    {
                        result.SetSubCategory(Category.Lingerie);
                        break;
                    }
                case "Asian Girls":
                    {
                        result.SetSubCategory(Category.Asian);
                        break;
                    }
                case "Holidays":
                    {
                        result.SetSubCategory(Category.Holidays);
                        break;
                    }
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    {
                        result.SetSubCategory(Category.Fantasy);
                        break;
                    }
                case "Celebrity Fakes":
                    {
                        result.SetSubCategory(Category.CelebrityFakes);
                        break;
                    }
                case "Fetish Girls":
                    {
                        result.SetSubCategory(Category.Fetish);
                        break;
                    }
            }
            return result;
        }


        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {

            // z.B. "/celebrities/211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string href = node.Attributes["href"]?.Value;
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }

            //z.B. "211314"
            string id = GetID(href);
            //z.B. "https://ftopx.com/211314/0_0"
            string wallPage = $"{_url}{id}/0_0";
            //z.B. "https://ftopx.com/images/201712/ftopx.com_5a4482e5acc2d.jpg"
            string url = GetImageUrl($"{_url}{id}/0_0");




            HtmlNode imageNode = node.SelectSingleNode("./img");


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetContentCategory(categoryName),
                Tags = GetTagsFromTagString(imageNode?.Attributes["alt"]?.Value),
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


        protected string GetEntryCategory(string url, string categoryName)
        {
            string search = $"{url}---{categoryName}";

            switch (search)
            {
                case "http://ftopx.com/---Girls & Cars":
                    {
                        return "Autos";
                    }
                case "http://ftopx.com/---Girls & Bikes":
                    {
                        return "Motorräder";
                    }
                case "http://ftopx.com/---Fantasy Girls":
                case "http://ftopx.com/---3D & Vector Girls":
                    {
                        return "Fantasy";
                    }
                case "http://ftopx.com/---Celebrity Fakes":
                    {
                        return "Celebrityfakes";
                    }
                case "http://ftopx.com/---Fetish Girls":
                    {
                        return "Fetischgirls";
                    }
                default:
                    {
                        return "Girls";
                    }
            }


        }






        private string GetID(string href)
        {
            //z.B. "211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string id = href.Substring(href.LastIndexOf("/") + 1);
            //z.B. "211314"
            return id.Substring(0, id.IndexOf('-'));
        }


        private string GetImageUrl(string wallPage)
        {
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
