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
            CancellationToken cancellationToken,
            bool onlyNews)
            : base(startPage, lastPage, cancellationToken, onlyNews)
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


                Uri uri = new Uri(url);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }
            return result;

        }
        protected override string GetSiteUrlForCategory(CrawlOffer catJob)
        {
            if (catJob.CurrentPage == 1)
            {
                //TODO: sortierung bei erster Seite
                return catJob.CategoryUri.AbsoluteUri;
            }

            //z.B. "https://wallpaperscraft.com/catalog/girls/date/page2"       
            return $"{catJob.CategoryUri.AbsoluteUri}/date/page{catJob.CurrentPage}";
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='wallpaper_pre']";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "3D":
                    return new ContentCategory(Category.Fantasy_3D);
                case "Abstract":
                    return new ContentCategory(Category.Fantasy_Abstract);
                case "Animals":
                    return new ContentCategory(Category.Hobbies_Animals);
                case "Anime":
                    return new ContentCategory(Category.Fantasy_Anime);
                case "Brands":
                    return new ContentCategory(Category.Other_Brands);
                case "Cars":
                    return new ContentCategory(Category.Vehicle_Cars);
                case "City":
                    return new ContentCategory(Category.Environment_City);
                case "Fantasy":
                    return new ContentCategory(Category.Fantasy);
                case "Flowers":
                    return new ContentCategory(Category.Environment_Flowers);
                case "Food":
                    return new ContentCategory(Category.Hobbies_Food);
                case "Games":
                    return new ContentCategory(Category.Media_Games);
                case "Girls":
                    return new ContentCategory(Category.Girls);
                case "Hi-Tech":
                    return new ContentCategory(Category.Hobbies_HiTech);
                case "Holidays":
                    return new ContentCategory(Category.Other_Holidays);
                case "Macro":
                    return new ContentCategory(Category.Environment_Macro);
                case "Men":
                    return new ContentCategory(Category.Men);
                case "Movies":
                    return new ContentCategory(Category.Media_Movies);
                case "Music":
                    return new ContentCategory(Category.Media_Music);
                case "Nature":
                    return new ContentCategory(Category.Environment_Landscape);
                case "Other":
                    return new ContentCategory(Category.Other);
                case "Space":
                    return new ContentCategory(Category.Environment_Space);
                case "Sport":
                    return new ContentCategory(Category.Hobbies_Sport);
                case "Textures":
                    return new ContentCategory(Category.Other_Textures);
                case "TV Series":
                    return new ContentCategory(Category.Media_TVSeries);
                case "Vector":
                    return new ContentCategory(Category.Fantasy_Vector);
                default:
                    return null;

            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(new Uri(_url), node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node, "./a");
            source.DownloadDoc = source.GetDetailsDocFromNode(node, "./div[@class='pre_info']/div[@class='pre_size']/a");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='wb_preview']/a[@class='wd_zoom']/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='wb_preview']/a/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DownloadDoc, "//div[@class='wb_tags']/a", new Func<HtmlNode, string>(x => x.InnerText.Trim()));


            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }






    }
}
