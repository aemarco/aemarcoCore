using aemarco.Crawler.Core.Helpers;
using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class WallpaperCrawlerNakedYoungModels : WallpaperCrawlerBasis
    {


        private readonly Uri _uri = new Uri("https://hotnakedgirls.porn/");


        public WallpaperCrawlerNakedYoungModels(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        { }



        internal override SourceSite SourceSite => SourceSite.NakedYoungModels;


        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>();


            var newOffer = CreateCrawlOffer(
                "NakedYoungModels",
                new Uri(_uri, $"/new/"),
                new ContentCategory(Category.Girls));
            result.Add(newOffer);

            return result;
        }

        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://nakedyoungmodels.com/new/?page=1"
            return new Uri($"{catJob.CategoryUri.AbsoluteUri}?page={catJob.CurrentPage - 1}");
        }

        protected override string GetSearchStringGorEntryNodes()
        {
            return "//a[@class='T']";
        }

        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var linkToAlbum = node.Attributes["href"]?.Value;
            var albumName = node.InnerText;
            if (string.IsNullOrWhiteSpace(albumName) || string.IsNullOrWhiteSpace(linkToAlbum)) return false;

            var linkToAlbumUri = new Uri(linkToAlbum);
            var albumDoc = HtmlHelper.GetHtmlDocument(linkToAlbumUri);


            var entryNodes = albumDoc.DocumentNode.SelectNodes("//p[@class='img']/span[@class='download']/a");


            var album = new AlbumEntry(albumName);
            foreach (var entryNode in entryNodes)
            {

                var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName)
                {
                    //details
                    ImageUri = new Uri(entryNode.Attributes["href"].Value)
                };


                source.ThumbnailUri = source.ImageUri;
                (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
                source.ContentCategory = catJob.Category;
                source.Tags = new List<string>();
                source.AlbumName = albumName;

                var wallEntry = source.WallEntry;
                if (wallEntry != null)
                {
                    album.Entries.Add(wallEntry);
                }
            }


            if (album.Entries.Any())
            {
                AddAlbum(album, catJob);
            }
            return album.Entries.Any();
        }
    }
}
