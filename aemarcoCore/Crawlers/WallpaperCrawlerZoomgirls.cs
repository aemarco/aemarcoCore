using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerZoomgirls : BildCrawlerBasis
    {
        const string _url = "https://zoomgirls.net/";
        const string _siteName = "zoomgirls";



        public WallpaperCrawlerZoomgirls(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerZoomgirls(
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




            //z.B. "Latest Wallpapers"
            string text = "Latest Wallpapers";
            //z.B. "latest_wallpapers"
            string href = "latest_wallpapers";
            //z.B. "https://zoomgirls.net/latest_wallpapers"
            string url = $"{_url}{href}";

            cats.Add(url, text);


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
                //z.B. "https://zoomgirls.net/latest_wallpapers/page/1"
                string pageUrl = $"{categoryUrl}/page/{page}";
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
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='thumb']/a");

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
            HtmlNode imageNode = node.SelectSingleNode("./img");

            // z.B. "amy-addison--wallpapers.html"
            string href = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }

            string docURL = _url + href;
            HtmlDocument doc = GetDocument(docURL);


            //z.B. "https://zoomgirls.net/wallpapers/amy-addison--1920x1200.jpg"
            string url = GetImageUrl(doc);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SiteCategory = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetEntryContentCategory(_siteName, categoryName),
                Tags = GetTags(doc),
                Url = url,
                ThumbnailUrl = $"{_url}{imageNode?.Attributes["src"]?.Value?.Substring(1)}",
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
            string fileName = url.Substring(url.LastIndexOf('/') + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private List<string> GetTags(HtmlDocument doc)
        {
            List<string> result = new List<string>();
            var nodes = doc.DocumentNode.SelectNodes("//ul[@class='tagcloud']/span/a");
            if (nodes == null)
            {
                return result;
            }

            foreach (var tagNode in nodes)
            {

                string entry = tagNode.InnerText.Trim();
                if (entry.Length > 0)
                {
                    result.Add(entry);
                }
            }

            return result;
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


            //z.B. "/view-jana-jordan--1920x1200.html"
            string url = targetNode.Attributes["href"]?.Value;
            //z.B. "jana-jordan--1920x1200.html"
            url = url.Substring(url.IndexOf("view") + 5);
            //z.B. "jana-jordan--1920x1200"
            url = url.Substring(0, url.IndexOf(".html"));
            //z.B. "https://zoomgirls.net/wallpapers/jana-jordan--1920x1200.jpg"
            url = _url + @"wallpapers/" + url + ".jpg";


            return url;

        }
    }
}
