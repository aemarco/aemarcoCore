using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers
{
    public class AdultWalls_Wallpaper : Crawler_Wallpaper
    {
        const string _url = "http://adultwalls.com/";
        const string _siteName = "adultwalls";



        public AdultWalls_Wallpaper(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(_siteName, progress, cancellationToken)
        {

        }
        public AdultWalls_Wallpaper(
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

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//ul[@role='menu']/li/a"))
            {

                //z.B. "Celebrities"
                string text = WebUtility.HtmlDecode(node.InnerText).Trim();
                if (String.IsNullOrEmpty(text) || text == "Sandbox")
                {
                    continue;
                }

                //z.B. "/celebrities/
                string href = node.Attributes["href"]?.Value;
                if (String.IsNullOrEmpty(href))
                {
                    continue;
                }

                //z.B. "celebrities/"
                href = href.Substring(1);
                //z.B. "https://ftopx.com/celebrities"
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
                //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
                string pageUrl = $"{categoryUrl}page/{page}/?sort=p.approvedAt&direction=desc";
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
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='thumbnail']/a");

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
            // z.B. "/celebrities/211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string href = node.Attributes["href"]?.Value;
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }

            //z.B. "211314"
            string id = GetID(href);
            //z.B. "https://ftopx.com/211314/0_0"
            string wallPage = $"{_url}{id}/0_0";
            //z.B. "https://ftopx.com/images/201712/ftopx.com_5a4482e5acc2d.jpg"
            string url = GetImageUrl($"{_url}{id}/0_0");


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry
            {
                SeitenKategorie = categoryName,
                Kategorie = GetEntryCategory(_url, categoryName),
                ContentCategory = GetEntryContentCategory(_siteName, categoryName),
                Tags = GetTags(node.SelectSingleNode("./img")?.Attributes["alt"]?.Value),
                Url = url,
                FileName = GetFileName(id),
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


        private string GetFileName(string id)
        {
            return $"{_siteName}_{id}";
        }

        private string GetID(string href)
        {
            //z.B. "211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string id = href.Substring(href.LastIndexOf("/") + 1);
            //z.B. "211314"
            return id.Substring(0, id.IndexOf('-'));
        }

        private List<string> GetTags(string tagString)
        {
            //z.B. "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"

            List<string> result = new List<string>();
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

        private string GetImageUrl(string wallPage)
        {
            HtmlDocument doc = GetDocument(wallPage);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@type='button']");
            if (node == null)
            {
                return null;
            }
            return node.Attributes["href"]?.Value;
        }





    }
}
