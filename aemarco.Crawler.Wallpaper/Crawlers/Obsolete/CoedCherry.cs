﻿//using aemarco.Crawler.Wallpaper.Common;
//using aemarco.Crawler.Wallpaper.Model;
//using HtmlAgilityPack;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;

//namespace aemarco.Crawler.Wallpaper.Crawlers.Obsolete;

////site announced going down by 2022


//[WallpaperCrawler("CoedCherry")]
//internal class WallpaperCrawlerCoedCherry : WallpaperCrawlerBasis
//{
//    private readonly Uri _uri = new("https://www.coedcherry.com/");


//    public WallpaperCrawlerCoedCherry(
//        // ReSharper disable once UnusedParameter.Local
//        int startPage,
//        // ReSharper disable once UnusedParameter.Local
//        int lastPage,
//        bool onlyNews)
//        : base(1, 1, onlyNews)
//    { }

//    protected override List<CrawlOffer> GetCrawlsOffers()
//    {
//        var result = new List<CrawlOffer>();

//        var cats = new List<string>
//        {

//            "Alternative",
//            "Amateur",
//            "Artistic",
//            "Ass",
//            "Asian",
//            "Babe",
//            "Barefoot",
//            "Bed",
//            "Beautiful",
//            "Big tits",
//            "Bikini",
//            "Black hair",
//            "Blonde",
//            "Blowjob",
//            "Boobs",
//            "Bra",
//            "Brunette",
//            "Busty",
//            "Close up",
//            "College",
//            "Cum",
//            "Cute",
//            "Dildo",
//            "Dress",
//            "Facial",
//            "Feet",
//            "Fingers",
//            "Flashing",
//            "Flat chested",
//            "Girlfriend",
//            "Glam",
//            "Group",
//            "Hardcore",
//            "Heels",
//            "Homemade",
//            "Latina",
//            "Lesbian",
//            "Lightspeed",
//            "Lingerie",
//            "Long hair",
//            "Long legs",
//            "Panties",
//            "Softcore",
//            "Masturbation",
//            "Model",
//            "Natural tits",
//            "Non nude",
//            "Nude",
//            "Outside",
//            "Painted toes",
//            "Petite",
//            "Pigtails",
//            "Public",
//            "Pussy",
//            "Pov",
//            "Redhead",
//            "Selfpics",
//            "Sexy",
//            "Shaved pussy",
//            "Shoes",
//            "Skinny",
//            "Small tits",
//            "Socks",
//            "Soles",
//            "Solo",
//            "Spreading",
//            "Striptease",
//            "Stockings",
//            "Tattoo",
//            "Thong",
//            "Tits",
//            "Tight",
//            "Topless",
//            "Toy",
//            "Upskirt",
//            "Wet",
//            "Vibrator"
//        };

//        foreach (var cat in cats)
//        {
//            var cc = GetContentCategory(cat);
//            if (cc is null)
//                continue;


//            var popularUri = new Uri(_uri, $"/galleries?tags={cat.ToLower().Replace(" ", "-")}");
//            var popularOffer = CreateCrawlOffer(cat, popularUri, cc);
//            result.Add(popularOffer);

//            var newUri = new Uri(_uri, $"/galleries?tags={cat.ToLower().Replace(" ", "-")}&sort=newest");
//            var newOffer = CreateCrawlOffer(cat, newUri, cc);
//            result.Add(newOffer);
//        }
//        return result;
//    }
//    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
//    {
//        //z.B. "https://www.coedcherry.com/galleries?tags=artistic&sort=newest"
//        return catJob.CategoryUri;
//    }
//    protected override string GetSearchStringGorEntryNodes()
//    {
//        return "//div[@id='search-results']/div[@class='thumbs ']/figure/a";
//    }

//    protected override ContentCategory DefaultCategory => new(Category.Girls);

//    protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
//    {
//        var linkToAlbum = node.Attributes["href"]?.Value;
//        var albumName = node.Attributes["title"].Value;
//        if (string.IsNullOrWhiteSpace(albumName)) return false;

//        var linkToAlbumUri = new Uri(_uri, linkToAlbum);
//        var albumDoc = HtmlHelper.GetHtmlDocument(linkToAlbumUri, 800, 2000);


//        var entryNodes = albumDoc.DocumentNode.SelectNodes("//div[@class='thumbs ']/figure/a");

//        List<string> tags = null;
//        var album = new AlbumEntry(albumName);
//        foreach (var entryNode in entryNodes)
//        {
//            //get tags during first node
//            var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);
//            if (entryNodes.IndexOf(entryNode) == 0)
//            {
//                tags = source.GetTagsFromNodes(
//                    albumDoc,
//                    "//a[@rel='tag']",
//                    x => WebUtility.HtmlDecode(x.InnerText)
//                        .Replace("#", string.Empty)
//                        .Replace("-", " ")
//                        .Trim());
//            }

//            //details
//            source.ImageUri = new Uri(entryNode.Attributes["href"].Value);
//            source.ThumbnailUri = source.ImageUri;
//            (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
//            source.ContentCategory = catJob.Category;
//            source.Tags = tags;
//            source.AlbumName = albumName;

//            var wallEntry = source.WallEntry;
//            if (wallEntry != null)
//            {
//                album.Entries.Add(wallEntry);
//            }
//        }


//        if (album.Entries.Any())
//        {
//            AddAlbum(album, catJob);
//        }
//        return album.Entries.Any();
//    }


//}