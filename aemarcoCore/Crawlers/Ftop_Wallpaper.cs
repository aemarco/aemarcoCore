using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public class Ftop_Wallpaper : Crawler_Wallpaper
    {
        const string _url = "http://ftopx.com/";
        const string _filePrefix = "ftopx";


        public override ICrawlerResult Start()
        {
            GetCategories();
            return OnCompleted();
        }

        public override async void StartAsync()
        {
            await Task.Factory.StartNew(Start);
        }

        public override async Task<ICrawlerResult> StartAsyncTask()
        {
            return await Task.Factory.StartNew(Start);
        }


        private void GetCategories()
        {

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


                GetCategory(url, text);
            }
        }

        private void GetCategory(string categoryUrl, string categoryName)
        {
            //starte bei 1
            int page = 1;
            bool pageContainsNews = true;
            do
            {
                //z.B. "http://ftopx.com/celebrities/page/1/?sort=p.approvedAt&direction=desc"
                string pageUrl = $"{categoryUrl}page/{page}/?sort=p.approvedAt&direction=desc";
                pageContainsNews = GetPage(pageUrl, categoryName);
                page++;

            } while (page <= 1 && pageContainsNews);

        }

        private bool GetPage(string pageUrl, string categoryName)
        {
            //Seite mit Wallpaperliste
            HtmlDocument doc = GetDocument(pageUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='thumbnail']/a");

            if (nodes == null || nodes.Count == 0)
            {
                return false;
            }


            //hier gibt es mindestens 1 Node. Neue Entries werden gezählt
            int count = 0;
            foreach (HtmlNode node in nodes)
            {
                if (AddWallEntry(node, categoryName))
                {
                    count++;
                }
            }

            //Die Seite muss mindestens 1 neuen Entry ergeben, ansonsten wird abgebrochen
            if (count == 0)
            {
                return false;
            }

            //Wenn mindestens 1 neues Entry dabei war geht es weiter
            return true;
        }

        private bool AddWallEntry(HtmlNode node, string categoryName)
        {
            // z.B. "/celebrities/211314-suzanne-a-metart-grafiti-wall-flowerdress.html"
            string href = node.Attributes["href"]?.Value;
            if (String.IsNullOrEmpty(href))
            {
                return false;
            }


            //jeder node = 1 Wallpaper
            WallEntry wallEntry = new WallEntry();
            wallEntry.SeitenKategorie = categoryName;
            wallEntry.Kategorie = GetEntryCategory(_url, categoryName);
            //z.B. Liste aus "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"
            wallEntry.Tags = GetTags(node.SelectSingleNode("./img")?.Attributes["alt"]?.Value);
            //z.B. "211314"
            string id = GetID(href);
            //z.B. "https://ftopx.com/211314/0_0"
            string wallPage = $"{_url}{id}/0_0";
            //z.B. "https://ftopx.com/images/201712/ftopx.com_5a4482e5acc2d.jpg"
            string url = GetImageUrl($"{_url}{id}/0_0");
            wallEntry.Url = url;
            wallEntry.FileName = GetFileName(id);
            wallEntry.Extension = Path.GetExtension(url);


            //Entry muss Url haben
            if (String.IsNullOrEmpty(wallEntry.Url))
            {
                return false;
            }

            //Neuer Entry
            if (AddEntry(wallEntry))
            {
                return true;
            }
            return false;
        }

        private string GetFileName(string id)
        {
            return $"{_filePrefix}_{id}";
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
