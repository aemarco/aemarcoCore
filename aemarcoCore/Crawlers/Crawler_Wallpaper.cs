using aemarcoCore.Crawlers.Types;
using aemarcoCore.Properties;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public abstract class Crawler_Wallpaper
    {
        private DirectoryInfo _dataPath;
        private List<string> _knownUrls;
        private CrawlerResult _result;


        public Crawler_Wallpaper()
        {
            LoadDataPath();
            LoadKnownUrls();
            _result = new CrawlerResult();
        }

        private void LoadDataPath()
        {
            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(Settings.Default.CrawlerData);
                if (!di.Exists) di.Create();

            }
            catch
            {
                di = new DirectoryInfo($"{Environment.CurrentDirectory}\\JSON");
                if (!di.Exists) di.Create();

            }
            finally
            {
                _dataPath = di;
            }
        }
        private void LoadKnownUrls()
        {
            string file = $"{_dataPath.FullName}\\known.json";
            if (File.Exists(file))
            {
                _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(file));
            }
            else
            {
                _knownUrls = new List<string>();
            }
        }



        public abstract ICrawlerResult Start();
        public abstract void StartAsync();
        public abstract Task<ICrawlerResult> StartAsyncTask();



        protected bool AddEntry(IWallEntry entry)
        {
            //Entry darf noch nicht bekannt sein
            if (_knownUrls.Contains(entry.Url))
            {
                return false;
            }

            //url als bekannte Url merken
            _knownUrls.Add(entry.Url);

            //Resultat befüllen
            _result.AddEntry(entry);


            OnNewEntry(entry);
            return true;
        }


        public event EventHandler<IWallEntry> NewEntry;
        protected virtual void OnNewEntry(IWallEntry entry)
        {
            if (NewEntry != null)
            {
                foreach (Delegate d in NewEntry.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, entry);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, entry });  // cleanup omitted
                    }
                }

            }
        }


        public event EventHandler<ICrawlerResult> Completed;
        protected virtual ICrawlerResult OnCompleted()
        {
            Save();

            if (Completed != null)
            {
                foreach (Delegate d in Completed.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, _result);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, _result });  // cleanup omitted
                    }
                }
            }
            return _result;
        }




        private void Save()
        {
            // save the List of known Urls so the Crawler may consider them next time            
            string file = $"{_dataPath.FullName}\\known.json";
            File.WriteAllText(file, JsonConvert.SerializeObject(
                _knownUrls.Distinct().ToList(),
                Formatting.Indented));


            // save result in a result file
            // z.B. \\\\nas\\web\\aemarcoCentral\\Core\\JSON\\21:36:41.result
            string resFile = $"{_dataPath.FullName}\\{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.result";
            File.WriteAllText(resFile, _result.GetJSON());

        }

        protected HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(url);
        }

        protected string GetEntryCategory(string url, string categoryName)
        {
            string search = $"{url}---{categoryName}";

            switch (search)
            {
                case "http://ftopx.com/---girls-cars":
                    {
                        return "Autos";
                    }
                case "http://ftopx.com/---girls-bikes":
                    {
                        return "Motorräder";
                    }
                case "http://ftopx.com/---fantasy-girls":
                case "http://ftopx.com/---vect":
                    {
                        return "Fantasy";
                    }
                case "http://ftopx.com/---celebrity-fakes":
                    {
                        return "Celebrityfakes";
                    }
                case "http://ftopx.com/---fetish-girls":
                    {
                        return "Fetischgirls";
                    }
                default:
                    {
                        return "Girls";
                    }
            }
        }





    }
}
