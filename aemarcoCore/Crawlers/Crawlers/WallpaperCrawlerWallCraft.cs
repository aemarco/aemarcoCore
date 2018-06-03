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
        private Uri _uri = new Uri("https://wallpaperscraft.com");

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
            var doc = GetDocument(_uri);
            var nodes = doc.DocumentNode.SelectNodes("//ul[@class='filters__list JS-Filters']/li/a");
            if (nodes == null) return result;
            foreach (HtmlNode node in nodes)
            {
                string text = WebUtility.HtmlDecode(node.FirstChild.InnerText).Trim();
                if (String.IsNullOrEmpty(text))
                {
                    continue;
                }

                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }
                Uri uri = new Uri(_uri, href);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }
            return result;

        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            if (catJob.CurrentPage == 1)
            {
                //z.B. "https://wallpaperscraft.com/catalog/girls/date"
                //return $"{catJob.CategoryUri.AbsoluteUri}/date";

                return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/date");
            }

            //z.B. "https://wallpaperscraft.com/catalog/girls/date/page2"       
            //return $"{catJob.CategoryUri.AbsoluteUri}/date/page{catJob.CurrentPage}";
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/date/page{catJob.CurrentPage}");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//li[@class='wallpapers__item']";
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
                    return new ContentCategory(Category.Girls_SFW);
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
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromNode(node, "./a");
            source.DownloadDoc = source.GetChildDocument(source.DetailsDoc, "//div[@class='wallpaper-table__row']/span[@class='wallpaper-table__cell']/a");


            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//img[@class='wallpaper__image']", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='wallpaper__image']", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DownloadDoc, "//div[@class='wallpaper__tags']/a", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));


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
