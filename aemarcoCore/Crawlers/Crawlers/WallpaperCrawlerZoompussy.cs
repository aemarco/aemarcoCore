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

            DownloadSingle(wallEntry);

            if (wallEntry.Filepath != null || File.Exists(wallEntry.Filepath))
            {
                AddEntry(wallEntry, catJob);
            }
            return true;
        }





        private void DownloadSingle(WallEntry wallEntry)
        {
            var target = GetTargetPath(wallEntry).FullName;
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
            var di = new DirectoryInfo(Path.Combine(app, "temp"));
            if (!di.Exists) di.Create();
            var targetPath = Path.Combine(di.FullName, $"{wallEntry.FileName}{wallEntry.Extension}");
            return new FileInfo(targetPath);
        }
        private void Download(WallEntry wallEntry, string target)
        {
            var referer = $"{_uri.AbsoluteUri}{wallEntry.FileName.ToLower()}";


            var httpRequest = (HttpWebRequest)WebRequest.Create(wallEntry.Url);
            httpRequest.Method = WebRequestMethods.Http.Get;
            httpRequest.Referer = referer;

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            // returned values are returned as a stream, then read into a string
            using (var httpResponseStream = httpResponse.GetResponseStream())
            {
                var bufferSize = 1024;
                var buffer = new byte[bufferSize];
                var bytesRead = 0;

                using (var fileStream = File.Create(target))
                {
                    while ((bytesRead = httpResponseStream.Read(buffer, 0, bufferSize)) != 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            var okay = false;
            using (var img = Image.FromFile(target))
            {
                if (img.Width > 0) okay = true;
            }

            if (!okay) File.Delete(target);

        }











    }

}
