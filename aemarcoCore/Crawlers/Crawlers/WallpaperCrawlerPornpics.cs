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
    internal class WallpaperCrawlerPornpics : WallpaperCrawlerBasis
    {
        private readonly Uri _uri = new Uri("https://www.pornpics.com/");

        internal override SourceSite SourceSite => SourceSite.Pornpics;

        public WallpaperCrawlerPornpics(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
            : base(1, 1, cancellationToken, onlyNews)
        {


        }

        protected override List<CrawlOffer> GetCrawlsOffers()
        {
            List<CrawlOffer> result = new List<CrawlOffer>();
            List<string> cats = new List<string>
            {
                "Amateur",
                "Anal", //HC
                "Anal Gape", //HC
                "Asian",
                "Ass",
                "Ass Fucking", //HC
                "Ass Licking", //HC
                "Babe",
                "Ball Licking", //HC
                "Bath",
                //"BBW",
                "Beach",
                "Big Cock", //HC
                "Big Tits",
                "Bikini",
                "Blindfold",
                "Blonde",
                "Blowbang", //HC
                "Blowjob", //HC
                "Bondage",
                "Boots",
                "Brunette",
                "Bukkake", //HC
                "Centerfold",
                //"CFNM",
                "Cheerleader",
                "Christmas",
                "Close Up",
                "Clothed",
                "College",
                "Cosplay",
                //"Cougar",
                "Cowgirl", //HC
                "Creampie", //HC
                "Cum In Mouth", //HC
                "Cum In Pussy", //HC
                "Cumshot", //HC
                "Cum Swapping", //HC
                "Deepthroat", //HC
                "Dildo",
                "Double Penetration", //HC
                "Ebony",
                "European",
                "Face",
                "Facesitting", //HC
                "Facial", //HC
                //"Femdom",
                "Fetish",
                "Fingering",
                "Fisting", //HC
                "Flexible",
                "Foot Fetish",
                "Footjob", //HC
                "Gangbang", //HC
                "Girlfriend",
                "Glamour",
                "Glasses",
                "Gloryhole", //HC
                //"Granny",
                "Groupsex", //HC
                "Gyno", //HC
                "Hairy",
                "Handjob", //HC
                "Hardcore", //HC
                "High Heels",
                "Homemade",
                //"Housewife",
                "Humping",
                "Indian",
                "Interracial", //HC
                "Japanese",
                "Jeans",
                "Kissing", //HC
                //"Ladyboy",
                "Latex",
                "Latina",
                "Legs",
                "Lesbian", //HC
                "Lingerie",
                "Maid",
                "Massage",
                "Masturbation",
                //"Mature",
                //"MILF",
                //"Mom",
                "Nipples",
                "Non Nude",
                "Nurse",
                "Office",
                "Oiled",
                "Orgy", //HC
                "Outdoor",
                "Panties",
                "Pantyhose",
                //"Party",
                //"Pegging",
                "Petite",
                "Piercing",
                "Pissing",
                "Police",
                "Pool",
                "Pornstar",
                "POV", //HC
                "Public",
                "Pussy",
                "Pussy Licking", //HC
                "Reality",
                "Redhead",
                //"Saggy Tits",
                "Schoolgirl",
                "Secretary", //HC
                "Seduction", //HC
                "Self Shot",
                "Shaved",
                //"Shemale",
                "Shorts",
                "Shower",
                "Skinny",
                "Skirt",
                "Smoking",
                "Socks",
                "Spanking",
                "Sports",
                "Spreading",
                "Squirting", //HC
                //"SSBBW",
                "Stockings",
                "Strapon", //HC
                "Stripper",
                "Tall",
                "Tattoo",
                "Teacher",
                "Teen",
                "Thai",
                "Threesome", //HC
                "Tiny Tits",
                "Titjob", //HC
                "Tribbing", //HC
                "Undressing",
                "Uniform",
                "Upskirt",
                "Voyeur",
                "Wedding", //HC
                "Wet",
                //"Wife",
                "Yoga Pants",

            };

            foreach (var cat in cats)
            {
                var offer = CreateCrawlOffer(cat, new Uri(_uri, $"/recent/{cat.ToLower().Replace(" ", "-")}/"), GetContentCategory(cat));
                result.Add(offer);
            }
            return result;
        }
        protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
        {
            //z.B. "http://babesunivers.com/wallpapers/lingerie-girls/1?order=publish-date-newest&resolution=all&search="                

            return catJob.CategoryUri;
        }
        protected override string GetSearchStringGorEntryNodes()
        {
            return "//li[@class='thumbwook']/a";
        }
        protected override IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Homemade":
                case "Amateur":
                    return new ContentCategory(Category.Girls_Amateur);
                case "Asian":
                case "Indian":
                case "Japanese":
                case "Thai":
                    return new ContentCategory(Category.Girls_Asian);
                case "Bondage":
                case "Blindfold":
                case "Latex":
                    return new ContentCategory(Category.Girls_Fetish);
                case "Christmas":
                    return new ContentCategory(Category.Girls_Holidays);
                case "Cosplay":
                    return new ContentCategory(Category.Girls_Cosplay);
                case "Gloryhole":
                    return new ContentCategory(Category.Girls_Gloryhole);
                case "Self Shot":
                    return new ContentCategory(Category.Girls_Selfies);


                case "Anal":
                case "Anal Gape":
                case "Ass Fucking":
                case "Ass Licking":
                case "Ball Licking":
                case "Big Cock":
                case "Blowbang":
                case "Blowjob":
                case "Bukkake":
                case "Cowgirl":
                case "Creampie":
                case "Cum In Mouth":
                case "Cum In Pussy":
                case "Cumshot":
                case "Cum Swapping":
                case "Deepthroat":
                case "Double Penetration":
                case "Facesitting":
                case "Facial":
                case "Fisting":
                case "Footjob":
                case "Gangbang":
                case "Groupsex":
                case "Gyno":
                case "Handjob":
                case "Hardcore":
                case "Interracial":
                case "Kissing":
                case "Lesbian":
                case "Orgy":
                case "POV":
                case "Pussy Licking":
                case "Secretary":
                case "Seduction":
                case "Squirting":
                case "Strapon":
                case "Threesome":
                case "Titjob":
                case "Tribbing":
                case "Wedding":
                    return new ContentCategory(Category.Girls_Hardcore);

                default:
                    return new ContentCategory(Category.Girls);

            }
        }
        protected override bool AddWallEntry(HtmlNode node, CrawlOffer catJob)
        {

            var linkToAlbum = node.Attributes["href"]?.Value;
            var linkToAlbumUri = new Uri(linkToAlbum);
            var albumDoc = GetDocument(linkToAlbumUri);

            var entryNodes = albumDoc.DocumentNode.SelectNodes("//li[@class='thumbwook']/a");

            var result = true;
            foreach (var entryNode in entryNodes)
            {
                var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

                List<string> tags = null;
                if (tags == null)
                {
                    tags = source.GetTagsFromNodes(albumDoc, "//div[@class='gallery-info__item tags']/a/span", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim()));
                    tags.AddRange(source.GetTagsFromNodes(albumDoc, "//div[@class='gallery-info__item']/a/span", new Func<HtmlNode, string>(x => WebUtility.HtmlDecode(x.InnerText).Trim())));
                }




                //details
                source.ImageUri = new Uri(entryNode.Attributes["href"].Value);
                source.ThumbnailUri = source.ImageUri;
                (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
                source.ContentCategory = catJob.Category;
                source.Tags = tags;


                WallEntry wallEntry = source.WallEntry;
                if (wallEntry == null)
                {
                    result = false;
                    continue;
                }
                AddEntry(wallEntry, catJob);
            }
            return result;
        }


    }
}
