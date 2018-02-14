using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerZoomgirls : WallpaperCrawlerBasis
    {
        const string _url = "https://zoomgirls.net/";

        public WallpaperCrawlerZoomgirls(
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

            //z.B. "Latest Wallpapers"
            string text = "Latest Wallpapers";
            //z.B. "latest_wallpapers"
            string href = "latest_wallpapers";
            //z.B. "https://zoomgirls.net/latest_wallpapers"
            Uri uri = new Uri($"{_url}{href}");
            IContentCategory cat = GetContentCategory(text);

            result.Add(CreateCrawlOffer(text, uri, cat));
            return result;
        }
        protected override string GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
            return $"{catJob.CategoryUri.AbsoluteUri}/page/{catJob.CurrentPage}";
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.Girls);
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(new Uri(_url), node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetDetailsDocFromNode(node);

            //details
            source.ImageUri = new Uri(GetImageUrl(source.DetailsDoc));
            source.ThumbnailUri = source.GetUriFromDocument(source.DetailsDoc, "//a[@class='wallpaper-thumb']/img", "src");
            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
            source.ContentCategory = GetContentCategory(catJob.SiteCategoryName);
            source.Tags = source.GetTagsFromNodes(source.DetailsDoc, "//ul[@class='tagcloud']/span/a", new Func<HtmlNode, string>(x => x.InnerText.Trim()));

            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }
        private string GetImageUrl(HtmlDocument doc)
        {
            HtmlNode targetNode = null;


            //select all resolution nodes
            var allNodes = doc.DocumentNode.SelectNodes("//div[@class='tagcloud']/span/a");
            if (allNodes == null)
            {
                return null;
            }
            //search for node with highest resolution
            int maxSum = 0;
            foreach (var node in allNodes)
            {
                //get both number values
                string[] txt = node.Attributes["title"]?.Value?.Split('x');
                if (txt != null && txt.Length == 2)
                {
                    int sum = 0;

                    try
                    {
                        //do the math
                        sum = int.Parse(txt[0].Trim()) * int.Parse(txt[1].Trim());
                    }
                    catch
                    {
                        continue;
                    }

                    //set to targetNode if sum is highest
                    if (sum > maxSum)
                    {
                        maxSum = sum;
                        targetNode = node;
                    }
                }
            }

            if (targetNode == null)
            {
                return null;
            }



            #region Image Url

            //z.B. "/view-jana-jordan--1920x1200.html"
            string url = targetNode.Attributes["href"]?.Value;
            //z.B. "jana-jordan--1920x1200.html"
            url = url.Substring(url.IndexOf("view") + 5);
            //z.B. "jana-jordan--1920x1200"
            url = url.Substring(0, url.IndexOf(".html"));
            //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"
            url = _url + @"wallpapers/" + url + ".jpg";

            #endregion

            return url;

        }


    }
}
