using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerAdultWalls : BildCrawlerBasis
    {
        const string _url = "http://adultwalls.com/";
        const string _siteName = "adultwalls";



        public WallpaperCrawlerAdultWalls(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerAdultWalls(
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

            //main page
            var doc = GetDocument(_url);

            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//li[@class='sub-menu-item']/a"))
            {

                //z.B. "Erotic Wallpapers"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text))
                {
                    continue;
                }

                //z.B. "/wallpapers/erotic-wallpapers"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "wallpapers/erotic-wallpapers"
                href = href.Substring(1);
                //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers"
                string url = $"{_url}{href}/";

                cats.Add(url, text);
            }

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
                //z.B. "http://adultwalls.com/wallpapers/erotic-wallpapers/1?order=publish-date-newest&resolution=all&search="                
                string pageUrl = $"{categoryUrl}{page}?order=publish-date-newest&resolution=all&search=";
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
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='thumb-container']/a");
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
            // z.B. "wallpaper/shot-jeans-topless-brunette-model"
            string detailsHref = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(detailsHref))
            {
                return false;
            }

            HtmlDocument detailsDoc = GetDocument($"{_url}{detailsHref}");

            //z.B. "http://adultwalls.com/web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string url = GetImageUrl(detailsDoc);


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SeitenKategorie = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetEntryContentCategory(_siteName, categoryName),
                Tags = GetTags(detailsDoc),
                Url = url,
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
            string name = url.Substring(url.IndexOf("papers/") + 7);
            name = name.Substring(0, name.IndexOf("/"));
            return name;
        }


        private List<string> GetTags(HtmlDocument doc)
        {

            List<string> result = new List<string>();
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='col-md-12']/a");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    string entry = node.InnerText.Trim();
                    if (entry.Length > 0)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }


        private string GetImageUrl(HtmlDocument doc)
        {
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@class='btn btn-danger']");
            if (node == null)
            {
                return null;
            }

            // z.B. "wallpaper/shot-jeans-topless-brunette-model/1920x1080"
            string href = node.Attributes["href"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(href))
            {
                return null;
            }

            //z.B. "http://adultwalls.com/wallpaper/shot-jeans-topless-brunette-model/1920x1080"
            string page = $"{_url}{href}";


            HtmlDocument doc2 = GetDocument(page);
            HtmlNode urlnode = doc2.DocumentNode.SelectSingleNode("//div[@class='wallpaper-preview-container']/a/img");
            if (urlnode == null)
            {
                return null;
            }

            // z.B. "web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string urlHref = urlnode.Attributes["src"]?.Value?.Substring(1);
            if (String.IsNullOrEmpty(urlHref))
            {
                return null;
            }

            // z.B. "http://adultwalls.com/web/wallpapers/shot-jeans-topless-brunette-model/1920x1080.jpg"
            string url = _url + urlHref;

            return url;
        }





    }
}
