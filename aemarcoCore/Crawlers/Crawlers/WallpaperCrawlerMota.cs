﻿using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
#pragma warning disable CRR0043 // Unused type
    internal class WallpaperCrawlerMota : WallpaperCrawlerBasis
    {
        public WallpaperCrawlerMota(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }


        private readonly Uri _uri = new Uri("https://motaen.com");
        private readonly Uri _erouri = new Uri("https://ero.motaen.com");
        internal override SourceSite SourceSite => SourceSite.Mota;


        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            //main page
            var doc = GetDocument(_uri);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='root-menu__flex']/li/a"))
            {

                //z.B. "World"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrWhiteSpace(text) || text == "Sandbox" || text == "All categories")
                {
                    continue;
                }

                //z.B. "/categories/view/name/world"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                Uri targetUri = null;
                if (node.Attributes["class"]?.Value == "erotica")
                {
                    targetUri = _erouri;
                }
                else
                {
                    targetUri = _uri;
                }


                //z.B. "https://motaen.com/categories/view/name/world"
                //z.B. "https://ero.motaen.com/categories/view/name/erotica"
                Uri uri = new Uri(targetUri, href);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "3D graphics":
                    return new ContentCategory(Common.Category.Fantasy_Art, 0, 0);
                case "Animals":
                    return new ContentCategory(Common.Category.Hobbies_Animals, 0, 0);
                case "Anime":
                    return new ContentCategory(Common.Category.Girls_Fantasy, 1, 19);
                case "Aviation":
                    return new ContentCategory(Common.Category.Vehicle_Planes, 0, 0);
                case "Cars":
                    return new ContentCategory(Common.Category.Vehicle_Cars, 0, 0);
                case "Celebrities":
                    return new ContentCategory(Common.Category.Girls_Celebrities, 1, 19);
                case "Food":
                    return new ContentCategory(Common.Category.Hobbies_Food, 0, 0);
                case "Games":
                    return new ContentCategory(Common.Category.Media_Games, 0, 0);
                case "Girls":
                    return new ContentCategory(Common.Category.Girls, 1, 19);
                case "Holidays":
                    return new ContentCategory(Common.Category.Other_Holidays, 0, 0);
                case "Men":
                    return new ContentCategory(Common.Category.Men, 1, 19);
                case "Motorcycles":
                    return new ContentCategory(Common.Category.Vehicle_Bikes, 0, 0);
                case "Movies":
                    return new ContentCategory(Common.Category.Media_Movies, 0, 0);
                case "Music":
                    return new ContentCategory(Common.Category.Media_Music, 0, 0);
                case "Nature":
                    return new ContentCategory(Common.Category.Environment, 0, 0);
                case "Space":
                    return new ContentCategory(Common.Category.Environment_Space, 0, 0);
                case "Sport":
                    return new ContentCategory(Common.Category.Hobbies_Sport, 0, 0);
                case "Various":
                    return new ContentCategory(Common.Category.Other, 0, 0);
                case "World":
                    return new ContentCategory(Common.Category.Environment_City, 0, 0);
                case "Erotica (18+)":
                    return new ContentCategory(Common.Category.Girls);
                default:
                    return new ContentCategory(Common.Category.Other);
            }
        }

        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://motaen.com/categories/view/name/male-celebrities"                
            string href = catJob.CategoryUri.AbsolutePath;
            href = href.Insert(href.IndexOf("view/") + 5, $"page/{catJob.CurrentPage}/order/date/");
            return new Uri(catJob.CategoryUri, href);
        }
        protected override string GetSearchStringGorEntryNodes()
        {

            return "//ul[@class='element']/li/a";
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var hrefTest = node.Attributes["href"]?.Value;
            if (String.IsNullOrWhiteSpace(hrefTest) || !hrefTest.StartsWith("/"))
                return false;



            var source = new WallEntrySource(catJob.CategoryUri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();

            var detailLinkNode = source.DetailsDoc?.DocumentNode?.SelectNodes("//div[@class='download-wallpaper']/ul").ToList()
                                    .First(x => x.InnerText.Contains("Download original"))
                                    .ChildNodes
                                    .First(x => x.FirstChild.Name == "a");
            source.DownloadDoc = source.GetChildDocumentFromNode(detailLinkNode, "./a");


            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='full-img col-md-9']/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='desk-img']/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@id='tags-container']/li/a", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName, source.Tags);

            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }

        protected IContentCategory GetContentCategory(string categoryName, List<string> tags)
        {
            var result = GetContentCategory(categoryName);
            if (tags == null)
                return result;

            if (result.SuggestedMinAdultLevel == 0 && result.SuggestedMaxAdultLevel == 0)
            {
                if (tags.Contains("girl") && !tags.Contains("child"))
                {
                    if (result.Category == Common.Category.Vehicle_Cars.ToString())
                        result = new ContentCategory(Common.Category.Girls_Cars, 1, 19);
                    else if (result.Category == Common.Category.Vehicle_Bikes.ToString())
                        result = new ContentCategory(Common.Category.Girls_Bikes, 1, 19);
                    else if (result.Category == Common.Category.Fantasy_Art.ToString())
                        result = new ContentCategory(Common.Category.Girls_Fantasy, 1, 19);
                    else
                    {
                        result.SuggestedMinAdultLevel = 1;
                        result.SuggestedMaxAdultLevel = 19;
                    }
                }
                else if (tags.Contains("flowers"))
                {
                    if (result.Category == Common.Category.Environment.ToString())
                        result = new ContentCategory(Common.Category.Environment_Flowers, 0, 0);
                }
            }
            else if (result.SuggestedMinAdultLevel == -1 && result.SuggestedMaxAdultLevel == -1)
            {
                if (tags.Contains("car") && result.Category == Common.Category.Girls.ToString())
                    result = new ContentCategory(Common.Category.Girls_Cars);
                else if (tags.Contains("motorcycle") && result.Category == Common.Category.Girls.ToString())
                    result = new ContentCategory(Common.Category.Girls_Bikes);
                else if ((tags.Contains("beach") || tags.Contains("sea") || tags.Contains("lake")) && result.Category == Common.Category.Girls.ToString())
                    result = new ContentCategory(Common.Category.Girls_Beaches);
                else if (tags.Contains("fetish") && result.Category == Common.Category.Girls.ToString())
                    result = new ContentCategory(Common.Category.Girls_Fetish);
            }


            return result;
        }


    }
}
