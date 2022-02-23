using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using aemarco.Crawler.Wallpaper.Base;
using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using HtmlAgilityPack;

namespace aemarco.Crawler.Wallpaper.Crawlers.Obsolete
{

    //site no longer exists
    [WallpaperCrawler("BabesUnivers", false)]
    internal class WallpaperCrawlerBabesUnivers : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://babesunivers.com/");



        public WallpaperCrawlerBabesUnivers(
            int startPage,
            int lastPage,
            bool onlyNews)
            : base(startPage, lastPage, onlyNews)
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

                //z.B. "http://babesunivers.com/wallpapers/lingerie-girls"
                var uri = new Uri(_uri, href);
                var cat = GetContentCategory(text);
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
        protected override ContentCategory DefaultCategory => new ContentCategory(Category.Girls);
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
