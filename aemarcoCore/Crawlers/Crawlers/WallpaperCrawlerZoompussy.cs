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
    class WallpaperCrawlerZoompussy : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("http://zoompussy.com/");

        internal override SourceSite SourceSite => SourceSite.Zoompussy;

        public WallpaperCrawlerZoompussy(
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
            List<string> cats = new List<string>
            {
                "asian",
                "lingerie",
                "stockings",
                "ass",
                "bikini",
                "blonde",
                "boobs",
                "ebony",
                "tits",
                "brunette",
                "legs",
                "models",
                "naked",
                "nude",
                "pussy",
                "redhead"
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
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {

                case "asian":
                    return new ContentCategory(Category.Girls_Asian);
                case "lingerie":
                case "stockings":
                    return new ContentCategory(Category.Girls_Lingerie);
                default:
                    return new ContentCategory(Category.Girls);
            }
        }
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


            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }


            //damn.... no fun without referer --> special treatment needed :(

            DownloadSingle(wallEntry);

            if (wallEntry.Filepath != null || File.Exists(wallEntry.Filepath))
            {
                AddEntry(wallEntry, catJob);
            }
            return true;
        }





        private void DownloadSingle(WallEntry wallEntry)
        {
            string target = GetTargetPath(wallEntry).FullName;
            try
            {
                if (!File.Exists(target))
                {
                    Download(wallEntry, target);
                }
                wallEntry.Filepath = target;
            }
            catch { }
        }
        private FileInfo GetTargetPath(WallEntry wallEntry)
        {
            var app = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(Path.Combine(app, "temp"));
            if (!di.Exists) di.Create();
            string targetPath = Path.Combine(di.FullName, $"{wallEntry.FileName}{wallEntry.Extension}");
            return new FileInfo(targetPath);
        }
        private void Download(WallEntry wallEntry, string target)
        {
            var referer = $"{_uri.AbsoluteUri}{wallEntry.FileName.ToLower()}";


            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(wallEntry.Url);
            httpRequest.Method = WebRequestMethods.Http.Get;
            httpRequest.Referer = referer;

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            // returned values are returned as a stream, then read into a string
            using (Stream httpResponseStream = httpResponse.GetResponseStream())
            {
                int bufferSize = 1024;
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;

                using (FileStream fileStream = File.Create(target))
                {
                    while ((bytesRead = httpResponseStream.Read(buffer, 0, bufferSize)) != 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            bool okay = false;
            using (var img = Image.FromFile(target))
            {
                if (img.Width > 0) okay = true;
            }

            if (!okay) File.Delete(target);

        }











    }
}
