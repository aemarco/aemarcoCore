using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;

namespace aemarco.Crawler.Wallpaper.Crawlers
{

    [WallpaperCrawler("Wallhaven")]
    internal class WallpaperCrawlerWallhaven : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://wallhaven.cc");


        public WallpaperCrawlerWallhaven(
            int startPage,
            int lastPage,
            bool onlyNews)
            : base(startPage, lastPage, onlyNews)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>
            {

                CreateCrawlOffer(
                "Anime_SFW",
                new Uri(_uri, @"search?q=&categories=010&purity=100&sorting=date_added&order=desc"),
                GetContentCategory("Anime_SFW")),

                CreateCrawlOffer(
                "Anime_Sketchy",
                new Uri(_uri, @"search?q=&categories=010&purity=010&sorting=date_added&order=desc"),
                GetContentCategory("Anime_Sketchy")),

                CreateCrawlOffer(
                "People_SFW",
                new Uri(_uri, @"search?q=&categories=001&purity=100&sorting=date_added&order=desc"),
                GetContentCategory("People_SFW")),

                CreateCrawlOffer(
                "People_Sketchy",
                new Uri(_uri, @"search?q=&categories=001&purity=010&sorting=date_added&order=desc"),
                GetContentCategory("People_Sketchy"))


            };
            return result;
        }


        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {

            //z.B. "https://alpha.wallhaven.cc/search?q=&categories=001&purity=010&sorting=date_added&order=desc&page=1"
            return new Uri($"{catJob.CategoryUri.AbsoluteUri}&page={catJob.CurrentPage}");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//ul/li/figure";

            //return "//div[@class='thumb']/a";
        }
        protected override ContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "Anime_SFW":
                    return new ContentCategory(Category.Girls_Fantasy, 1, 19);
                case "Anime_Sketchy":
                    return new ContentCategory(Category.Girls_Fantasy);
                case "People_SFW":
                    return new ContentCategory(Category.Girls, 1, 19);
                case "People_Sketchy":
                    return new ContentCategory(Category.Girls);
            }
            return DefaultCategory;
        }
        protected override ContentCategory DefaultCategory => new ContentCategory(Category.Girls);


        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);
            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode("./a[@class='preview']");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@id='wallpaper']", "src");
            source.ThumbnailUri = new Uri(_uri, source.GetSubNodeAttribute(node, "data-src", "./img[@alt='loading']"));
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@id='tags']/li", x => WebUtility.HtmlDecode(x.InnerText).Trim());
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);




            var wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }




    }
}
