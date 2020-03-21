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

    internal class WallpaperCrawlerWallhaven : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://wallhaven.cc");

        internal override SourceSite SourceSite => SourceSite.Wallhaven;

        public WallpaperCrawlerWallhaven(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>
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
        protected override IContentCategory DefaultCategory => new ContentCategory(Category.Girls);


        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);
            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode("./a[@class='preview']");

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
