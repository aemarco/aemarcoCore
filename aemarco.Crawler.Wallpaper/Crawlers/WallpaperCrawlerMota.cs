using aemarco.Crawler.Wallpaper.Base;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace aemarco.Crawler.Wallpaper.Crawlers
{

    [WallpaperCrawler("Mota", true)]
    internal class WallpaperCrawlerMota : WallpaperCrawlerBasis
    {
        public WallpaperCrawlerMota(
            int startPage,
            int lastPage,
            bool onlyNews)
            : base(startPage, lastPage, onlyNews)
        {

        }


        private readonly Uri _uri = new Uri("https://motaen.com");
        private readonly Uri _erouri = new Uri("https://ero.motaen.com");


        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>();

            //main page
            var doc = HtmlHelper.GetHtmlDocument(_uri);

            foreach (var node in doc.DocumentNode.SelectNodes("//ul[@class='root-menu__flex']/li/a"))
            {

                //z.B. "World"
                var text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (string.IsNullOrWhiteSpace(text) || text == "Sandbox" || text == "All categories")
                {
                    continue;
                }

                //z.B. "/categories/view/name/world"
                var href = node.Attributes["href"]?.Value;
                if (string.IsNullOrEmpty(href))
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
                var uri = new Uri(targetUri, href);
                var cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override ContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "3D graphics":
                    return new ContentCategory(Category.Fantasy_Art, 0, 0);
                case "Animals":
                    return new ContentCategory(Category.Hobbies_Animals, 0, 0);
                case "Anime":
                    return new ContentCategory(Category.Girls_Fantasy, 1, 19);
                case "Aviation":
                    return new ContentCategory(Category.Vehicle_Planes, 0, 0);
                case "Cars":
                    return new ContentCategory(Category.Vehicle_Cars, 0, 0);
                case "Celebrities":
                    return new ContentCategory(Category.Girls_Celebrities, 1, 19);
                case "Food":
                    return new ContentCategory(Category.Hobbies_Food, 0, 0);
                case "Games":
                    return new ContentCategory(Category.Media_Games, 0, 0);
                case "Girls":
                    return new ContentCategory(Category.Girls, 1, 19);
                case "Holidays":
                    return new ContentCategory(Category.Other_Holidays, 0, 0);
                case "Men":
                    return new ContentCategory(Category.Men, 1, 19);
                case "Motorcycles":
                    return new ContentCategory(Category.Vehicle_Bikes, 0, 0);
                case "Movies":
                    return new ContentCategory(Category.Media_Movies, 0, 0);
                case "Music":
                    return new ContentCategory(Category.Media_Music, 0, 0);
                case "Nature":
                    return new ContentCategory(Category.Environment, 0, 0);
                case "Space":
                    return new ContentCategory(Category.Environment_Space, 0, 0);
                case "Sport":
                    return new ContentCategory(Category.Hobbies_Sport, 0, 0);
                case "Various":
                    return new ContentCategory(Category.Other, 0, 0);
                case "World":
                    return new ContentCategory(Category.Environment_City, 0, 0);
                case "Erotica (18+)":
                    return new ContentCategory(Category.Girls);
            }
            return DefaultCategory;
        }
        protected override ContentCategory DefaultCategory => new ContentCategory(Category.Other);

        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://motaen.com/categories/view/name/male-celebrities"                
            var href = catJob.CategoryUri.AbsolutePath;
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
            if (string.IsNullOrWhiteSpace(hrefTest) || !hrefTest.StartsWith("/"))
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

            var wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }

        protected ContentCategory GetContentCategory(string categoryName, List<string> tags)
        {
            var result = GetContentCategory(categoryName);
            if (tags == null)
                return result;

            if (result.SuggestedMinAdultLevel == 0 && result.SuggestedMaxAdultLevel == 0)
            {
                if (tags.Contains("girl") && !tags.Contains("child"))
                {
                    if (result.Category == Category.Vehicle_Cars.ToString())
                        result = new ContentCategory(Category.Girls_Cars, 1, 19);
                    else if (result.Category == Category.Vehicle_Bikes.ToString())
                        result = new ContentCategory(Category.Girls_Bikes, 1, 19);
                    else if (result.Category == Category.Fantasy_Art.ToString())
                        result = new ContentCategory(Category.Girls_Fantasy, 1, 19);
                    else
                    {
                        result.SuggestedMinAdultLevel = 1;
                        result.SuggestedMaxAdultLevel = 19;
                    }
                }
                else if (tags.Contains("flowers"))
                {
                    if (result.Category == Category.Environment.ToString())
                        result = new ContentCategory(Category.Environment_Flowers, 0, 0);
                }
            }
            else if (result.SuggestedMinAdultLevel == -1 && result.SuggestedMaxAdultLevel == -1)
            {
                if (tags.Contains("car") && result.Category == Category.Girls.ToString())
                    result = new ContentCategory(Category.Girls_Cars);
                else if (tags.Contains("motorcycle") && result.Category == Category.Girls.ToString())
                    result = new ContentCategory(Category.Girls_Bikes);
                else if ((tags.Contains("beach") || tags.Contains("sea") || tags.Contains("lake")) && result.Category == Category.Girls.ToString())
                    result = new ContentCategory(Category.Girls_Beaches);
                else if (tags.Contains("fetish") && result.Category == Category.Girls.ToString())
                    result = new ContentCategory(Category.Girls_Fetish);
            }


            return result;
        }


    }
}
