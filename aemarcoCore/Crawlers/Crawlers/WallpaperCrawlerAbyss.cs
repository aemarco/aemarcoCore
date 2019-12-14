using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers
{
#pragma warning disable CRR0043 // Unused type
    internal class WallpaperCrawlerAbyss : WallpaperCrawlerBasis
    {
        private int _queryCount = -1;
        private readonly Uri _uri = new Uri("https://wall.alphacoders.com");
        private readonly Uri _muri = new Uri("https://mobile.alphacoders.com");

        internal override SourceSite SourceSite => SourceSite.Abyss;

        public WallpaperCrawlerAbyss(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
            : base(startPage, lastPage, cancellationToken, onlyNews)
        {

        }

        private string QueryApi(params string[] parameters)
        {
            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category_list

            List<string> allParams = parameters.ToList();
            allParams.Insert(0, $"auth={ConfigurationHelper.AbyssAPI_Key}");


            string queryUri = $"{_uri.AbsoluteUri}/api2.0/get.php";
            string call = $"{queryUri}?{string.Join("&", allParams)}";


            var wr = WebRequest.Create(call);
            string result = null;
            using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }

                }
            }
            if (String.IsNullOrWhiteSpace(result)) throw new Exception("Abyss No Query Result");
            return result;
        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();

            if (String.IsNullOrWhiteSpace(ConfigurationHelper.AbyssAPI_Key))
                return result;
            try
            {
                //check how much querycounts are left
                var qc = QueryApi("method=query_count");
                var qcObj = JsonConvert.DeserializeObject<AbyssQueryCount>(qc);
                _queryCount = qcObj.counts.month_count;


                var str = QueryApi("method=category_list");
                var catResult = JsonConvert.DeserializeObject<AbyssCategoryList>(str);
                if (catResult.success)
                {
                    _abyssCats = catResult.categories.ToList();
                    foreach (var cat in catResult.categories)
                    {
                        if (GetContentCategory(cat.name) is IContentCategory cc && _queryCount + 350 < 150000)
                        {

                            //wallpaper from WallpaperAbyss
                            _queryCount += 350;
                            var uri = new Uri(cat.url);
                            var crawloffer = CreateCrawlOffer(cat.name, uri, cc);
                            crawloffer.CrawlMethod = CrawlMethod.API;
                            crawloffer.ReportNumberOfEntriesPerPage(30);
                            result.Add(crawloffer);


                            var muri = new Uri(_muri, $"/by-category/{cat.id}");
                            var mOffer = CreateCrawlOffer(cat.name, muri, cc);
                            result.Add(mOffer);
                        }
                    }
                }
            }
            catch { }

            return result;

        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Women":
                    return new ContentCategory(Common.Category.Girls, 1, 29);

                default:
                    return null;
            }
        }

        #region api

        private List<AbyssCategory> _abyssCats;

        protected override bool HandleAPIPage(CrawlOffer catJob, CancellationToken cancellationToken, Action progAction)
        {
            var result = false;

            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category&id=10&page=10&info_level=2
            var pageString = QueryApi(
                "method=category",
                $"id={_abyssCats.First(x => x.name == catJob.SiteCategoryName).id}",
                $"page={catJob.CurrentPage}",
                "check_last=1");
            var page = JsonConvert.DeserializeObject<AbyssWallpaperPage>(pageString);
            if (!page.success || page.wallpapers.Count() == 0) return result;


            foreach (var w in page.wallpapers)
            {
                if (cancellationToken.IsCancellationRequested ||
                        catJob.ReachedMaximumStreak)
                {
                    catJob.ReportEndReached();
                    return true;
                }
                if (AddWallEntry(catJob, w))
                {
                    result = true;
                }
                progAction();
            }
            catJob.ReportPageDone();

            if (page.is_last) return false;


            //valid Page contains minimum 1 valid Entry
            return result;
        }

        private bool AddWallEntry(CrawlOffer catJob, AbyssWallpaper wall)
        {
            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=wallpaper_info&id=595064
            var wallInfoString = QueryApi(
                "method=wallpaper_info",
                $"id={wall.id}");
            var wallInfo = JsonConvert.DeserializeObject<AbyssWallpaperInfoResult>(wallInfoString);
            if (!wallInfo.success) return false;
            var w = wallInfo.wallpaper;


            string file = $"Abyss_{w.category}_{w.sub_category}_{w.id}";
            string extension = $".{w.file_type}";
            List<string> tags = wallInfo.tags.Select(x => x.name).ToList();
            if (!String.IsNullOrWhiteSpace(w.sub_category)) tags.Insert(0, w.sub_category);
            var cc = CheckForRealCategory(catJob.Category, tags, w);


            var entry = new WallEntry(
                w.url_image,
                w.url_thumb,
                file,
                extension,
                cc,
                catJob.SiteCategoryName,
                tags);


            if (!entry.IsValid)
            {
                return false;
            }
            AddEntry(entry, catJob);
            return true;
        }

        #endregion

        #region normal

        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "https://mobile.alphacoders.com/by-category/33?page=1"
            return new Uri(catJob.CategoryUri, $"{catJob.CategoryUri.AbsolutePath}?page={catJob.CurrentPage}");

        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//div[@class='thumb-container']/div[@class='container-masonry']/div[@class='item-element']/a";
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {
            var source = new WallEntrySource(_muri, node, catJob.SiteCategoryName);

            //docs
            source.DetailsDoc = source.GetChildDocumentFromRootNode(null);


            //details
            source.ImageUri = source.GetUriFromDocument(source.DetailsDoc, "//img[@class='img-full-size']", "src");

            var thumbUrl = source.GetSubNodeAttribute(source.RootNode, "src", "./img[@class='img-responsive']");
            source.ThumbnailUri = new Uri(thumbUrl);

            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri, catJob.SiteCategoryName);


            source.Tags = source.GetTagsFromNode(source.RootNode, "title", "./img[@class='img-responsive']");
            source.ContentCategory = CheckForRealCategory(catJob.Category, source.Tags, null);


            WallEntry wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }

        #endregion



        private IContentCategory CheckForRealCategory(IContentCategory cat, List<string> tags, AbyssWallpaperInfo info)
        {
            //api walls
            if (info != null)
            {
                if (info.category == "Women")
                {
                    switch (info.sub_category)
                    {
                        case "Asian":
                            return new ContentCategory(Category.Girls_Asian, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                        case "Girls & Cars":
                            return new ContentCategory(Category.Girls_Cars, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                        case "Cosplay":
                            return new ContentCategory(Category.Girls_Cosplay, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                        case "Girls & Motorcycles":
                            return new ContentCategory(Category.Girls_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                        case "Bikini":
                            return new ContentCategory(Category.Girls_Beaches, 20, cat.SuggestedMaxAdultLevel);

                        case "Model":
                            return new ContentCategory(Category.Girls_Celebrities, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

                        case "Girls & Guns":
                            return new ContentCategory(Category.Girls_Guns, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                    }
                }



            }


            //all walls
            if (tags != null && tags.Any() && cat.MainCategory == "Girls")
            {
                if (tags.Any(x => x.ToLower().Contains("asian")))
                    return new ContentCategory(Category.Girls_Asian, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & cars")))
                    return new ContentCategory(Category.Girls_Cars, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("cosplay")))
                    return new ContentCategory(Category.Girls_Cosplay, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & motorcycles")))
                    return new ContentCategory(Category.Girls_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("bikini")))
                    return new ContentCategory(Category.Girls_Beaches, 20, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("model")))
                    return new ContentCategory(Category.Girls_Celebrities, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                if (tags.Any(x => x.ToLower().Contains("girls & guns")))
                    return new ContentCategory(Category.Girls_Guns, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);

            }



            return cat;
        }


    }
#pragma warning restore CRR0043 // Unused type
}
