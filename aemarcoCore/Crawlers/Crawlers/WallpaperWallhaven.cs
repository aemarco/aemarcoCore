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
    internal class WallpaperWallhaven : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://alpha.wallhaven.cc");


        public WallpaperWallhaven(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
            : base(startPage, lastPage, cancellationToken, onlyNews)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>
            {
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
            return new Uri($"{catJob.CategoryUri.AbsoluteUri}&page={ catJob.CurrentPage }");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//ul/li/figure";

            //return "//div[@class='thumb']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {

            switch (categoryName)
            {
                case "Anime_Sketchy":
                    return new ContentCategory(Category.Girls_Fantasy);
                case "People_SFW":
                    return new ContentCategory(Category.Girls_SFW, 10, 19);
                case "People_Sketchy":
                    return new ContentCategory(Category.Girls);



                default:
                    return new ContentCategory(Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);
            //docs
            source.DetailsDoc = source.GetChildDocumentFromNode(node, "./a[@class='preview']");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@id='wallpaper']", "src");
            source.ThumbnailUri = new Uri(_uri, source.GetSubNodeAttribute(node, "data-src", "./img[@alt='loading']"));
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@id='tags']/li", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);




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
