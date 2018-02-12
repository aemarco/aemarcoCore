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
        protected override IContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "Animals":
                    return new ContentCategory(Category.Animals);
                case "Cars":
                    return new ContentCategory(Category.Vehicle_Cars);
                case "City":
                    return new ContentCategory(Category.Environment_City);
                case "Fantasy":
                    return new ContentCategory(Category.Fantasy);
                case "Flowers":
                    return new ContentCategory(Category.Environment_Flowers);
                case "Games":
                    return new ContentCategory(Category.Media_Games);
                case "Girls":
                    return new ContentCategory(Category.Girls);
                case "Holidays":
                    return new ContentCategory(Category.Holidays);
                case "Men":
                    return new ContentCategory(Category.Men);
                case "Movies":
                    return new ContentCategory(Category.Media_Movies);
                case "Music":
                    return new ContentCategory(Category.Media_Music);
                case "Nature":
                    return new ContentCategory(Category.Environment_Landscape);
                case "Space":
                    return new ContentCategory(Category.Environment_Space);
                case "Sport":
                    return new ContentCategory(Category.Sport);
                case "TV Series":
                    return new ContentCategory(Category.Media_TVSeries);
                default:
                    return null;

            }
        }
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            var source = new WallEntrySource(new Uri(_url), node, categoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node, "./a");
            source.DownloadDoc = source.GetDetailsDocFromNode(node, "./div[@class='pre_info']/div[@class='pre_size']/a");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='wb_preview']/a[@class='wd_zoom']/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='wb_preview']/a/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, categoryName);
            source.ContentCategory = GetContentCategory(categoryName);
            source.Tags = source.GetTagsFromNodes(source.DownloadDoc, "//div[@class='wb_tags']/a", new Func<HtmlNode, string>(x => x.InnerText.Trim()));


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
