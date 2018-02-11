using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerWallCraft : WallpaperCrawlerBasis
    {
        const string _url = "https://wallpaperscraft.com/";


        public WallpaperCrawlerWallCraft(
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

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='left_category']/li/a"))
            {
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "All" || text == "Wallpapers for Android")
                {
                    continue;
                }

                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }
                string url = $"{_url.Substring(0, _url.IndexOf("//"))}{href}";

                result.Add(url, text);
            }
            return result;
        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            //main page
            var doc = GetDocument(_url);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='left_category']/li/a"))
            {
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "All" || text == "Wallpapers for Android")
                {
                    continue;
                }

                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }
                string url = $"{_url.Substring(0, _url.IndexOf("//"))}{href}";



                IContentCategory cat = GetContentCategory(text);


                CrawlOffer offer = new CrawlOffer
                {
                    Name = text,
                    Url = url,
                    MainCategory = cat?.MainCategory,
                    SubCategory = cat?.SubCategory
                };

                result.Add(offer);
            }
            return result;

        }

        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            if (page == 1)
            {
                return categoryUrl;
            }

            //z.B. "https://wallpaperscraft.com/catalog/girls/date/page2"       
            return $"{categoryUrl}/date/page{page}";
        }

        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='wallpaper_pre']";
        }

        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            var infoNode = node?.SelectSingleNode("./div[@class='pre_info']/div[@class='pre_size']/a");
            if (infoNode == null)
            {
                return false;
            }
            //z.B. "https://wallpaperscraft.com/download/girl_winter_hat_funny_118618/5767x3845"
            string docURL = $"{_url.Substring(0, _url.IndexOf("//"))}{infoNode.Attributes["href"]?.Value}";
            if (String.IsNullOrEmpty(docURL))
            {
                return false;
            }
            HtmlDocument doc = GetDocument(docURL);

            //z.B. "https://wallpaperscraft.com/image/diane_kruger_actress_blonde_face_make_up_109818_1920x1200.jpg"
            var imageUrl = GetImageUrls(doc);
            if (String.IsNullOrEmpty(imageUrl))
            {
                return false;
            }

            var detailsNode = node?.SelectSingleNode("./a");
            string detailsUrl = $"{_url.Substring(0, _url.IndexOf("//"))}{detailsNode.Attributes["href"]?.Value}";
            var detailsDoc = GetDocument(detailsUrl);
            var imageNode = detailsDoc?.DocumentNode.SelectSingleNode("//div[@class='wb_preview']/a/img");
            var thumbnailUrl = $"{_url.Substring(0, _url.IndexOf("//"))}{imageNode.Attributes["src"]?.Value}";




            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
                (
                imageUrl,
                thumbnailUrl,
                GetFileName(imageUrl, string.Empty),
                GetContentCategory(categoryName),
                categoryName,
                GetTagsFromNodes(doc.DocumentNode.SelectNodes("//div[@class='wb_tags']/a"))
                );

            //Entry muss valid sein
            if (!wallEntry.IsValid)
            {
                return false;
            }

            AddEntry(wallEntry);
            return true;

        }


        protected override IContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "Girls":
                    return new ContentCategory(Category.Girls);
                case "Animals":
                    return new ContentCategory(Category.Animals);
                case "Cars":
                    return new ContentCategory(Category.Cars);
                case "City":
                    return new ContentCategory(Category.Environment_City);
                case "Fantasy":
                    return new ContentCategory(Category.Fantasy);
                case "Flowers":
                    return new ContentCategory(Category.Environment_Flowers);
                case "Games":
                    return new ContentCategory(Category.Games);
                case "Holidays":
                    return new ContentCategory(Category.Holidays);
                case "Men":
                    return new ContentCategory(Category.Men);
                case "Movies":
                    return new ContentCategory(Category.Movies);
                case "Music":
                    return new ContentCategory(Category.Music);
                case "Nature":
                    return new ContentCategory(Category.Environment_Landscape);
                case "Space":
                    return new ContentCategory(Category.Environment_Space);
                case "Sport":
                    return new ContentCategory(Category.Sport);
                case "TV Series":
                    return new ContentCategory(Category.TVSeries);
                default:
                    return null;

            }
        }



        private string GetImageUrls(HtmlDocument doc)
        {
            HtmlNode targetNode = doc.DocumentNode.SelectSingleNode("//div[@class='wb_preview']/a[@class='wd_zoom']/img");
            if (targetNode == null)
            {
                return null;
            }
            //z.B. "https://wallpaperscraft.com/image/girl_winter_hat_funny_118618_5767x3845.jpg"
            string url = $"{_url.Substring(0, _url.IndexOf("//"))}{targetNode.Attributes["src"]?.Value}";

            return url;

        }


    }
}
