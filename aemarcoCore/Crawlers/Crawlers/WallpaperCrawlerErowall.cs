using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
#pragma warning disable CRR0043 // Unused type
    internal class WallpaperCrawlerErowall : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://erowall.com");

        internal override SourceSite SourceSite => SourceSite.Erowall;

        public WallpaperCrawlerErowall(
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
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='m']/li[@class='m']/a"))
            {

                //z.B. "#brunette"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || !text.StartsWith("#"))
                {
                    continue;
                }
                else
                {
                    //z.B. "brunette"
                    text = text.Substring(1);
                    if (String.IsNullOrEmpty(text))
                    {
                        continue;
                    }
                    //z.B. "Brunette"
                    text = char.ToUpper(text[0]) + text.Substring(1);
                }



                //z.B. "/search/brunette/"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "search/brunette/"
                href = href.Replace("search", "teg");


                //z.B. "https://erowall.com/teg/brunette/"
                Uri uri = new Uri(_uri, href);
                IContentCategory cat = GetContentCategory(text);
                result.Add(CreateCrawlOffer(text, uri, cat));
            }

            return result;

        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://erowall.com/teg/brunette/page/1"       
            //return $"{catJob.CategoryUri.AbsoluteUri}page/{catJob.CurrentPage}";
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}page/{ catJob.CurrentPage }");
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='wpmini']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Lesbians":
                    return new ContentCategory(Common.Category.Girls_Lesbians);
                case "Lingerie":
                    return new ContentCategory(Common.Category.Girls_Lingerie);
                case "Beach":
                    return new ContentCategory(Common.Category.Girls_Beaches);
                case "Asian":
                    return new ContentCategory(Common.Category.Girls_Asian);
                case "Anime":
                    return new ContentCategory(Common.Category.Girls_Fantasy);
                default:
                    return new ContentCategory(Common.Category.Girls);
            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            //details
            Match match = Regex.Match(node.Attributes["href"]?.Value, @"/(\d+)/$");
            // z.B. "24741"
            string imageLink = match.Groups[1].Value;

            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode();

            //details
            source.ImageUri = new Uri(_uri, $"/wallpapers/original/{imageLink}.jpg");
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//div[@class='view-left']/a/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);
            source.ContentCategory = catJob.Category;
            source.Tags = source.GetTagsFromNode(node, "title");


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
