using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerPornomass : BildCrawlerBasis
    {

        const string _url = "http://pornomass.com/";
        const string _url2 = "http://gif.pornomass.com/";
        const string _siteName = "pornomass";
        const string _siteName2 = "gifpornomass";



        public WallpaperCrawlerPornomass(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerPornomass(
            int startPage,
            int lastPage,
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, startPage, lastPage, progress, cancellationToken)
        {

        }





        protected override void DoWork()
        {
            GetCategories();
        }

        private void GetCategories()
        {

            Dictionary<string, string> cats = new Dictionary<string, string>();


            cats.Add(_url, "Pornomass");
            cats.Add(_url2, "Gifpornomass");


            ReportNumberOfCategories(cats.Count);

            foreach (string cat in cats.Keys)
            {
                if (!IShallGoAheadWithCategories())
                {
                    break;
                }
                GetCategory(cat, cats[cat]);
            }


        }



        private void GetCategory(string categoryUrl, string categoryName)
        {
            int page = GetStartingPage();
            if (page == 0) page = 1;


            bool pageValid = true;
            do
            {
                //z.B. "http://pornomass.com/page/1"
                string pageUrl = $"{categoryUrl}page/{page}";
                pageValid = GetPage(pageUrl, categoryName);
                page++;


                //} while (page <= 1 && pageContainsNews);
            } while (pageValid && IShallGoAheadWithPages(page));
            ReportCategoryDone();
        }



        /// <summary>
        /// return true if page contains minimum 1 valid Entry
        /// </summary>        
        private bool GetPage(string pageUrl, string categoryName)
        {
            bool result = false;
            //Seite mit Wallpaperliste
            HtmlDocument doc = GetDocument(pageUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='fit-box']/a[@class='fit-wrapper']");

            //non Valid Page
            if (nodes == null || nodes.Count == 0)
            {
                return result;
            }

            ReportNumberOfEntries(nodes.Count);

            foreach (HtmlNode node in nodes)
            {
                if (!IShallGoAheadWithEntries())
                {
                    result = false;
                    break;
                }

                if (AddWallEntry(node, categoryName))
                {
                    result = true;
                }
                ReportEntryDone();
            }

            //valid Page contains minimum 1 valid Entry
            ReportPageDone();
            return result;
        }





        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        private bool AddWallEntry(HtmlNode node, string categoryName)
        {

            // z.B. "photo/1949-xxx.html" -- "Pornomass"
            // z.B. "photo/614-beautiful-girl-anal-gif.html" -- "Gifpornomass"
            string href = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }

            string site = string.Empty;
            string siteName = string.Empty;
            string thumbnail = string.Empty;
            if (categoryName == "Pornomass")
            {
                site = _url;
                siteName = _siteName;
                HtmlNode imageNode = node.SelectSingleNode("./img");
                thumbnail = $"{site}{imageNode?.Attributes["src"]?.Value?.Substring(1)}";
            }
            else
            {
                site = _url2;
                siteName = _siteName2;
                HtmlNode imageNode = node.SelectSingleNode("./video");
                thumbnail = $"{site}{imageNode?.Attributes["poster"]?.Value?.Substring(1)}";
            }

            


            // z.B. "http://pornomass.com/uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            // z.B. "http://gif.pornomass.com/uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            string url = GetImageUrl(site, href);


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetEntryContentCategory(siteName, categoryName),
                Tags = new List<string>(),
                Url = url,
                ThumbnailUrl = thumbnail,
                FileName = GetFileName(url),
                Extension = FileExtension.GetFileExtension(url)
            };



            //Entry muss valid sein
            if (!IsValidEntry(wallEntry))
            {
                return false;
            }

            AddEntry(wallEntry);

            return true;
        }


        private string GetFileName(string url)
        {
            //z.B. "http://pornomass.com/uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            //z.B. "http://gif.pornomass.com/uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"

            //z.B. "1949-xxx.jpg" -- "Pornomass"
            //z.B. "614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            string fileName = url.Substring(url.LastIndexOf("/") + 1);
            //z.B. "1949-xxx" -- "Pornomass"
            //z.B. "614-beautiful-girl-anal-gif" -- "Gifpornomass"
            return Path.GetFileNameWithoutExtension(fileName); ;
        }

        private string GetImageUrl(string site, string href)
        {
            //z.B. "http://pornomass.com/photo/1949-xxx.html" -- "Pornomass"
            //z.B. "http://gif.pornomass.com/photo/614-beautiful-girl-anal-gif.html" -- "Gifpornomass"
            string targetUrl = $"{site}{href}";

            HtmlDocument doc = GetDocument(targetUrl);
            HtmlNode targetNode = doc.DocumentNode.SelectSingleNode("//a[@class='photo-blink']");
            if (targetNode == null)
            {
                return null;
            }

            //z.B. "uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            //z.B. "uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            string targetHref = targetNode.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(targetHref))
            {
                return null;
            }

            //z.B. "http://pornomass.com/uploads/photo/original/1949-xxx.jpg" -- "Pornomass"
            //z.B. "http://gif.pornomass.com/uploads/photo/original/614-beautiful-girl-anal-gif.gif" -- "Gifpornomass"
            return $"{site}{targetHref}";



        }





    }
}
