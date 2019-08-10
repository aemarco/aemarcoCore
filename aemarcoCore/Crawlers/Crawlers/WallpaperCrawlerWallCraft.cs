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
#pragma warning disable CRR0043 // Unused type
    internal class WallpaperCrawlerWallCraft : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://wallpaperscraft.com");

        internal override SourceSite SourceSite => SourceSite.Wallpaperscraft;

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
                if (String.IsNullOrEmpty(href) || href.StartsWith("http"))
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
                    return new ContentCategory(Common.Category.Fantasy_3D, 0, 0);
                case "Abstract":
                    return new ContentCategory(Common.Category.Fantasy_Abstract, 0, 0);
                case "Animals":
                    return new ContentCategory(Common.Category.Hobbies_Animals, 0, 0);
                case "Anime":
                    return new ContentCategory(Common.Category.Fantasy_Anime, 0, 0);
                case "Art":
                    return new ContentCategory(Common.Category.Fantasy_Art, 0, 0);
                case "Cars":
                    return new ContentCategory(Common.Category.Vehicle_Cars, 0, 0);
                case "City":
                    return new ContentCategory(Common.Category.Environment_City, 0, 0);
                case "Fantasy":
                    return new ContentCategory(Common.Category.Fantasy, 0, 0);
                case "Flowers":
                    return new ContentCategory(Common.Category.Environment_Flowers, 0, 0);
                case "Food":
                    return new ContentCategory(Common.Category.Hobbies_Food, 0, 0);
                case "Games":
                    return new ContentCategory(Common.Category.Media_Games, 0, 0);
                case "Technologies":
                    return new ContentCategory(Common.Category.Hobbies_HiTech, 0, 0);
                case "Holidays":
                    return new ContentCategory(Common.Category.Other_Holidays, 0, 0);
                case "Macro":
                    return new ContentCategory(Common.Category.Environment_Macro, 0, 0);
                case "Motorcycles":
                    return new ContentCategory(Common.Category.Vehicle_Bikes, 0, 0);
                case "Movies":
                    return new ContentCategory(Common.Category.Media_Movies, 0, 0);
                case "Music":
                    return new ContentCategory(Common.Category.Media_Music, 0, 0);
                case "Nature":
                    return new ContentCategory(Common.Category.Environment_Landscape, 0, 0);
                case "Other":
                    return new ContentCategory(Common.Category.Other, 0, 0);
                case "Space":
                    return new ContentCategory(Common.Category.Environment_Space, 0, 0);
                case "Sport":
                    return new ContentCategory(Common.Category.Hobbies_Sport, 0, 0);
                case "Textures":
                    return new ContentCategory(Common.Category.Other_Textures, 0, 0);
                case "TV Series":
                    return new ContentCategory(Common.Category.Media_TVSeries, 0, 0);
                case "Vector":
                    return new ContentCategory(Common.Category.Fantasy_Vector, 0, 0);
                case "Words":
                    return new ContentCategory(Common.Category.Other_Words, 0, 0);
                default:
                    return null;

            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {


            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode("./a");
            source.DownloadDoc = source.GetChildDocumentFromDocument(source.DetailsDoc, "//div[@class='wallpaper-table__row']/span[@class='wallpaper-table__cell']/a");


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
