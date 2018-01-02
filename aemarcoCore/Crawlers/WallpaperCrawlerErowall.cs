using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;


namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawlerErowall : BildCrawlerBasis
    {
        const string _url = "https://erowall.com/";
        const string _siteName = "erowall";



        public WallpaperCrawlerErowall(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public WallpaperCrawlerErowall(
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
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@class='m']/li[@class='m']/a"))
            {

                //z.B. "#brunette"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || !text.StartsWith("#"))
                {
                    continue;
                }
                else
                {
                    //z.B. "brunette"
                    text = text.Substring(1);                    
                    if (String.IsNullOrEmpty(text))
                    {
                        continue;
                    }
                    //z.B. "Brunette"
                    text = char.ToUpper(text[0]) + text.Substring(1);
                }



                //z.B. "/search/brunette/"
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "search/brunette/"
                href = href.Substring(1).Replace("search","teg");


                //z.B. "https://erowall.com/search/brunette/"
                string url = $"{_url}{href}";

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
                //z.B. "https://erowall.com/teg/brunette/page/1"                
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
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='wpmini']/a");
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
            // z.B. "/w/24741/"
            string href = node.Attributes["href"]?.Value;
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }

            // z.B. "https://erowall.com//wallpapers/original/24741.jpg"
            string url = GetImageUrl(href);
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }



            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SeitenKategorie = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetEntryContentCategory(_siteName, categoryName),
                Tags = GetTags(node),
                Url = url,
                FileName = GetFileName(url, categoryName),
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


        private string GetFileName(string url, string categoryName)
        {
            string fileName =  categoryName + "_" + url.Substring(url.LastIndexOf("/") + 1);
            return Path.GetFileNameWithoutExtension(fileName);
        }


        private List<string> GetTags(HtmlNode node)
        {
            List<string> result = new List<string>();

            string tagString = node.Attributes["title"]?.Value;            
            if (String.IsNullOrEmpty(tagString))
            {
                return result;
            }
            else
            {
                string[] tags = tagString.Split(',');
                foreach (string tag in tags)
                {
                    //z.B. "flowerdress"
                    string entry = tag.Trim();
                    if (entry.Length > 0)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }


        private string GetImageUrl(string href)
        {
            Match match = Regex.Match(href, @"/(\d+)/$");
            // z.B. "24741"
            string imageLink = match.Groups[1].Value;
            // z.B. "https://erowall.com//wallpapers/original/24741.jpg"
            string url = _url + "/wallpapers/original/" + imageLink + ".jpg";

            return url;
        }


    }
}
