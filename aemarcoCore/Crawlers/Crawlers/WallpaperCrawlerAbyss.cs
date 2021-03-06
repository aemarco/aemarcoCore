﻿using aemarcoCore.Common;
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

    internal class WallpaperCrawlerAbyss : WallpaperCrawlerBasis
    {
        private int _queryCount = -1;
        private readonly Uri _uri = new Uri("https://wall.alphacoders.com");
        private readonly Uri _muri = new Uri("https://mobile.alphacoders.com");

        internal override SourceSite SourceSite => SourceSite.Abyss;

        public WallpaperCrawlerAbyss(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
            : base(startPage, lastPage, onlyNews, cancellationToken)
        {

        }



        private List<AbyssCategory> _abyssCats;
        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            var result = new List<CrawlOffer>();

            if (string.IsNullOrWhiteSpace(ConfigurationHelper.AbyssApiKey))
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
                        if (GetContentCategory(cat.name) is IContentCategory cc)
                        {
                            //add api stuff
                            if (_queryCount + 350 < 150000)
                            {
                                //wallpaper from WallpaperAbyss
                                _queryCount += 350;
                                var uri = new Uri(cat.url);
                                var crawloffer = CreateCrawlOffer(cat.name, uri, cc);
                                crawloffer.CrawlMethod = CrawlMethod.API;
                                crawloffer.ReportNumberOfEntriesPerPage(30);
                                result.Add(crawloffer);
                            }

                            //add classic sites
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
                    return new ContentCategory(Category.Girls, 1, 29);
                case "Vehicles":
                    return new ContentCategory(Category.Vehicle, 0, 0);
                case "Video Game":
                    return new ContentCategory(Category.Media_Games, 0, 0);
                case "Men":
                    return new ContentCategory(Category.Men, 0, 10);
                case "Movie":
                    return new ContentCategory(Category.Media_Movies, 0, 0);
                case "Music":
                    return new ContentCategory(Category.Media_Music, 0, 0);
                case "TV Show":
                    return new ContentCategory(Category.Media_TVSeries, 0, 0);
            }
            return DefaultCategory;
        }


        protected override bool GetPage(CrawlOffer catJob)
        {
            switch (catJob.CrawlMethod)
            {
                case CrawlMethod.Classic:
                    return base.GetPage(catJob);
                case CrawlMethod.API:
                    return HandleAbyssApiPage(catJob);
                default:
                    throw new NotImplementedException($"{catJob.CrawlMethod} not implemented.");
            }
        }

        #region api

        private bool HandleAbyssApiPage(CrawlOffer catJob)
        {
            var result = false;

            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category&id=10&page=10&info_level=2
            var pageString = QueryApi(
                "method=category",
                $"id={_abyssCats.First(x => x.name == catJob.SiteCategoryName).id}",
                $"page={catJob.CurrentPage}",
                "check_last=1");
            var page = JsonConvert.DeserializeObject<AbyssWallpaperPage>(pageString);
            if (!page.success || !page.wallpapers.Any()) return result;


            foreach (var w in page.wallpapers)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                if (catJob.ReachedMaximumStreak)
                {
                    catJob.ReportEndReached();
                    break;
                }

                if (AddWallEntry(catJob, w))
                {
                    result = true;
                }
                OnProgress();
            }

            catJob.ReportPageDone();
            if (page.is_last) catJob.ReportEndReached();

            //valid Page contains minimum 1 valid Entry
            return result;
        }

        private bool AddWallEntry(CrawlOffer catJob, AbyssWallpaper wall)
        {
            var source = new WallEntrySource();



            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=wallpaper_info&id=595064
            var wallInfoString = QueryApi(
                "method=wallpaper_info",
                $"id={wall.id}");
            var wallInfo = JsonConvert.DeserializeObject<AbyssWallpaperInfoResult>(wallInfoString);
            if (!wallInfo.success) return false;
            var wallInf = wallInfo.wallpaper;


            source.ImageUri = new Uri(wallInf.url_image);
            source.ThumbnailUri = new Uri(wallInf.url_thumb);
            source.Filename = $"Abyss_{wallInf.category}_{wallInf.sub_category}_{wallInf.id}";
            source.Extension = $".{wallInf.file_type}";
            source.Tags = wallInfo.tags.Select(x => x.name).ToList();
            source.ContentCategory = CheckForRealCategory(catJob.Category, source.Tags, wallInf);
            if (!string.IsNullOrWhiteSpace(wallInf.sub_category)) source.Tags.Insert(0, wallInf.sub_category);
            source.SiteCategory = catJob.SiteCategoryName;



            var wallEntry = source.WallEntry;
            if (wallEntry == null)
            {
                return false;
            }
            AddEntry(wallEntry, catJob);
            return true;
        }

        private string QueryApi(params string[] parameters)
        {
            // https://wall.alphacoders.com/api2.0/get.php?auth=YOUR_KEY&method=category_list

            var allParams = parameters.ToList();
            allParams.Insert(0, $"auth={ConfigurationHelper.AbyssApiKey}");


            var queryUri = $"{_uri.AbsoluteUri}/api2.0/get.php";
            var call = $"{queryUri}?{string.Join("&", allParams)}";


            var wr = WebRequest.Create(call);
            string result = null;
            using (var response = (HttpWebResponse)wr.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }

                }
            }
            if (string.IsNullOrWhiteSpace(result)) throw new Exception("Abyss No Query Result");
            return result;
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


            var wallEntry = source.WallEntry;
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
                else if (info.category == "Vehicles")
                {
                    switch (info.sub_category)
                    {
                        case "Motorcycle":
                            return new ContentCategory(Category.Vehicle_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                        case "Aircraft":
                            return new ContentCategory(Category.Vehicle_Planes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                    }

                }
            }




            //all walls
            if (tags != null && tags.Any())
            {
                if (cat.MainCategory == "Girls")
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
                else if (cat.MainCategory == "Vehicles")
                {
                    if (tags.Any(x => x.ToLower().Contains("motorcycle")))
                        return new ContentCategory(Category.Vehicle_Bikes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                    if (tags.Any(x => x.ToLower().Contains("aircraft")))
                        return new ContentCategory(Category.Vehicle_Planes, cat.SuggestedMinAdultLevel, cat.SuggestedMaxAdultLevel);
                }
            }

            return cat;
        }



    }

}
