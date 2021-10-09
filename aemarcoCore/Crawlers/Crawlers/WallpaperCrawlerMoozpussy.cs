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

    /// <summary>
    /// updated: 23.12.2019
    /// </summary>
    internal class WallpaperCrawlerMoozpussy : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://moozpussy.com/");

        internal override SourceSite SourceSite => SourceSite.Moozpussy;

        public WallpaperCrawlerMoozpussy(
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
            foreach (var node in doc.DocumentNode.SelectNodes("//article/blockquote/a"))
            {
                //z.B. "asian"
                var text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                //z.B. "/search/asian"
                var href = node.Attributes["href"]?.Value;
                if (string.IsNullOrEmpty(href) || href.StartsWith("http") || !href.StartsWith("/search"))
                {
                    continue;
                }


                //z.B. "https://moozpussy.com/search/asian/"
                var uri = new Uri(_uri, href);
                var cat = GetContentCategory(text);

                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://moozpussy.com/search/asian/page/1/"
            return new Uri($"{catJob.CategoryUri.AbsoluteUri}/page/{catJob.CurrentPage}");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//li/div[@class='thumb']/a";
        }

        protected override IContentCategory DefaultCategory => new ContentCategory(Category.Girls);
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {

            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();

            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/div[@class='cent']/a", "href");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/div[@class='cent']/a/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//div[@class='post_z']/a", new Func<HtmlNode, string>(x =>
            {
                if (x.Attributes["href"] != null &&
                    x.Attributes["href"].Value.StartsWith(_uri.AbsoluteUri))
                {
                    return WebUtility.HtmlDecode(x.InnerText).Trim();
                }
                return null;
            }));


            var wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }


            //damn.... no fun without referer --> special treatment needed :(
            //source.DownloadWithReferer(wallEntry, $"{_uri.AbsoluteUri}{wallEntry.FileName.ToLower()}");
            //if (!wallEntry.IsValid || string.IsNullOrWhiteSpace(wallEntry.FileContentAsBase64String)) 
            //    return false;

            if (!wallEntry.IsValid)
                return false;


            AddEntry(wallEntry, catJob);
            return true;
        }


    }

}
