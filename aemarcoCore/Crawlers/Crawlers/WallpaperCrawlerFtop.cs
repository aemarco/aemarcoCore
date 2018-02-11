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
    internal class WallpaperCrawlerFtop : WallpaperCrawlerBasis
    {
        const string _url = "https://ftopx.com/";


        public WallpaperCrawlerFtop(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(startPage, lastPage, cancellationToken)
        {

        }


        protected override Dictionary<string, string> GetCategoriesDict()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            //main page
            var doc = GetDocument(_url);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            {

                //z.B. "Celebrities"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "Sandbox")
                {
                    continue;
                }

                //z.B. "/celebrities/
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "celebrities/"
                href = href.Substring(1);
                //z.B. "https://ftopx.com/celebrities"
                string url = $"{_url}{href}";

                result.Add(url, text);
            }

            return result;
        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            //main page
            var doc = GetDocument(_url);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            {

                //z.B. "Celebrities"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "Sandbox")
                {
                    continue;
                }

                //z.B. "/celebrities/
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "celebrities/"
                href = href.Substring(1);
                //z.B. "https://ftopx.com/celebrities"
                string url = $"{_url}{href}";

                IContentCategory cat = GetContentCategory(text);

                CrawlOffer offer = new CrawlOffer
                {
                    Name = text,
                    Url = url,
                    MainCategory = cat.MainCategory,
                    SubCategory = cat.SubCategory
                };

                result.Add(offer);
            }

            return result;

        }

        protected override string GetSiteUrlForCategory(string categoryUrl, int page)
        {
            //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
            return $"{categoryUrl}page/{page}/?sort=p.approvedAt&direction=desc";
        }

        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumbnail']/a";
        }

        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Celebrities":
                    return new ContentCategory(Category.Girls_Celebrities);
                case "Girls & Beaches":
                    return new ContentCategory(Category.Girls_Beaches);
                case "Girls & Cars":
                    return new ContentCategory(Category.Girls_Cars);
                case "Girls & Bikes":
                    return new ContentCategory(Category.Girls_Bikes);
                case "Lingerie Girls":
                    return new ContentCategory(Category.Girls_Lingerie);
                case "Asian Girls":
                    return new ContentCategory(Category.Girls_Asian);
                case "Holidays":
                    return new ContentCategory(Category.Girls_Holidays);
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    return new ContentCategory(Category.Girls_Fantasy);
                case "Celebrity Fakes":
                    return new ContentCategory(Category.Girls_CelebrityFakes);
                case "Fetish Girls":
                    return new ContentCategory(Category.Girls_Fetish);
                default:
                    return new ContentCategory(Category.Girls);
            }
        }

        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected override bool AddWallEntry(HtmlNode node, string categoryName)
        {
            //z.B. "https://ftopx.com/images/201712/ftopx.com_5a4482e5acc2d.jpg"
            var (ThumbnailUrl, ImageUrl) = GetImageUrls(node.Attributes["href"]?.Value);

            if (String.IsNullOrEmpty(ImageUrl))
            {
                return false;
            }

            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
                (
                ImageUrl,
                ThumbnailUrl,
                GetFileName(ImageUrl, $"{categoryName}_"),
                GetContentCategory(categoryName),
                categoryName,
                GetTagsFromTagString(node.SelectSingleNode("./img")?.Attributes["alt"]?.Value)
                );

            //Entry muss valid sein
            if (!wallEntry.IsValid)
            {
                return false;
            }

            AddEntry(wallEntry);
            return true;
        }

        private (string ThumbnailUrl, string ImageUrl) GetImageUrls(string href)
        {
            if (String.IsNullOrEmpty(href))
            {
                return (null, null);
            }

            #region Photo Details site

            string detailsUrl = $"{_url}{href.Substring(1)}";
            var detailsDoc = GetDocument(detailsUrl);
            var imageNode = detailsDoc?.DocumentNode.SelectSingleNode("//img[@class='img-responsive img-rounded']");
            var thumbnailUrl = imageNode?.Attributes["src"]?.Value;

            #endregion

            #region Download Photo site

            //z.B. "211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string id = href.Substring(href.LastIndexOf("/") + 1);
            //z.B. "211314"
            id = id.Substring(0, id.IndexOf('-'));

            //z.B. "https://ftopx.com/211314/0_0"
            string wallPage = $"{_url}{id}/0_0";

            HtmlDocument doc = GetDocument(wallPage);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@type='button']");
            if (node == null)
            {
                return (thumbnailUrl, null);
            }
            string imageUrl = node.Attributes["href"]?.Value;

            #endregion

            return (thumbnailUrl, imageUrl);


        }


    }
}
