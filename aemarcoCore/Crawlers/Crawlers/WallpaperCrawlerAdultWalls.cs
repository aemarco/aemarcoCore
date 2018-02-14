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
        private Uri _uri = new Uri("http://adultwalls.com");

        public WallpaperCrawlerAdultWalls(
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

                //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers"
                Uri uri = new Uri(_uri, href);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers/1?order=publish-date-newest&resolution=all&search="                
            //return $"{catJob.CategoryUri.AbsoluteUri}/{catJob.CurrentPage}?order=publish-date-newest&resolution=all&search=";
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}/{catJob.CurrentPage}?order=publish-date-newest&resolution=all&search=");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb-container']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Lingerie Models":
                    return new ContentCategory(Category.Girls_Lingerie);
                default:
                    return new ContentCategory(Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node);
            source.DownloadDoc = source.GetChildDocument(source.DetailsDoc, "//a[@class='btn btn-danger']");

            //details
            source.ImageUri = source.GetUriFromDocument(source.DownloadDoc, "//div[@class='wallpaper-preview-container']/a/img", "src");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='img-rounded']", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, "wallpapers/", "/", catJob.SiteCategoryName);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='col-md-12']/a", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));


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
