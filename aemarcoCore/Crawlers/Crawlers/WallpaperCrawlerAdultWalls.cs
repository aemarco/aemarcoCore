using aemarco.Crawler.Core.Helpers;
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

    internal class WallpaperCrawlerAdultWalls : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://adultwalls.com");

        internal override SourceSite SourceSite => SourceSite.AdultWalls;

        public WallpaperCrawlerAdultWalls(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>();

            //main page
            var doc = HtmlHelper.GetHtmlDocument(_uri);

            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            foreach (var node in doc.DocumentNode.SelectNodes("//li[@class='sub-menu-item']/a"))
            {
                //z.B. "Erotic Wallpapers"
                var text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                //z.B. "/wallpapers/erotic-wallpapers"
                var href = node.Attributes["href"]?.Value;
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers"
                var uri = new Uri(_uri, href);
                var cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers/1?order=publish-date-newest&resolution=all&search="                
            //return $"{catJob.CategoryUri.AbsoluteUri}/{catJob.CurrentPage}?order=publish-date-newest&resolution=all&search=";
            
            var result = new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/{catJob.CurrentPage}?order=publish-date-newest&resolution=all&search=");
            
            return result;
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//a[@class='thumbnail clearfix']";
        }

        protected override IContentCategory DefaultCategory => new ContentCategory(Category.Girls);
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {

            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();
            var dNode = source.DetailsDoc.DocumentNode.SelectSingleNode("//*[text()[contains(., 'Original Resolution')]]");
            source.DownloadDoc = source.GetChildDocumentFromNode(dNode, "./a");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='wallpaper-preview-container']/a/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='box-main']/p/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, "wallpapers/", "/", catJob.SiteCategoryName);
            source.ContentCategory = catJob.Category;
            //source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='col-md-12']/a", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));
            source.Tags = new List<string>();

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
