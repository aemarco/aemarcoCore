using aemarcoCore.Common;
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

    internal class WallpaperCrawlerBabesUnivers : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://babesunivers.com/");

        internal override SourceSite SourceSite => SourceSite.BabesUnivers;

        public WallpaperCrawlerBabesUnivers(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            //main page
            var doc = GetDocument(_uri);

            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//li[@class='sub-menu-item']/a"))
            {
                //z.B. "Erotic Wallpapers"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text))
                {
                    continue;
                }

                //z.B. "/wallpapers/erotic-wallpapers"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "http://babesunivers.com/wallpapers/lingerie-girls"
                Uri uri = new Uri(_uri, href);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://babesunivers.com/wallpapers/lingerie-girls/1?order=publish-date-newest&resolution=all&search="                

            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/{catJob.CurrentPage}?order=publish-date-newest&resolution=all&search=");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//a[@class='thumbnail clearfix']";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Lingerie Girls":
                    return new ContentCategory(Common.Category.Girls_Lingerie);
                default:
                    return new ContentCategory(Common.Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {

            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();
            var detailLinkNode = source.DetailsDoc?.DocumentNode?.SelectNodes("//p/a").ToList()
                                    .First(x => x.ParentNode.InnerText.Contains("Original Resolution"));
            source.DownloadDoc = source.GetChildDocumentFromNode(detailLinkNode);


            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='wallpaper-preview-container']/a/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='box-main']/p/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, "wallpapers/", "/", catJob.SiteCategoryName);
            source.ContentCategory = catJob.Category;
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='col-md-4']/a[@class='btn btn-default btn-xs']", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));


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
