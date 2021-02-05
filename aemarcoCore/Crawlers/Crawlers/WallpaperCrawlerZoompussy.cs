using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{

    internal class WallpaperCrawlerZoompussy : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://zoompussy.com/");

        internal override SourceSite SourceSite => SourceSite.Zoompussy;

        public WallpaperCrawlerZoompussy(
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
            var cats = new List<string>
            {
                "asian",
                "ass",
                "bikini",
                "blonde",
                "boobs",
                "ebony",
                "tits",
                "brunette",
                "legs",
                "lingerie",
                "models",
                "naked",
                "nude",
                "pussy",
                "redhead",
                "stockings"
            };

            foreach (var cat in cats)
            {
                var offer = CreateCrawlOffer(cat, new Uri(_uri, $"/search/{cat}/"), GetContentCategory(cat));
                result.Add(offer);
            }
            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://zoompussy.com/search/asian/page/1/"
            return new Uri($"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}");
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
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/blockquote/a", "href");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@id='post_content']/blockquote/a/img", "src");
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
