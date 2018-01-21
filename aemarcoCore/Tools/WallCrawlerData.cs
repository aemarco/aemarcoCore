using aemarcoCore.Common;
using aemarcoCore.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aemarcoCore.Tools
{
    internal static class WallCrawlerData
    {
        private static bool _isInitiallized = false;
        private static DirectoryInfo _dataPath;
        private static List<string> _knownUrls;

        private static object _lock = new object();

        private static void Init()
        {

            //get path for saving
            try
            {
                _dataPath = new DirectoryInfo(Settings.Default.CrawlerData);
                if (!_dataPath.Exists) _dataPath.Create();
            }
            catch
            {
                _dataPath = new DirectoryInfo($"{Environment.CurrentDirectory}\\JSON");
                if (!_dataPath.Exists) _dataPath.Create();

            }

            //load already known entries
            string file = $"{_dataPath.FullName}\\known.json";
            if (File.Exists(file))
            {
                _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(file));
            }
            else
            {
                _knownUrls = new List<string>();
            }

            _isInitiallized = true;
        }


        internal static bool IsKnownEntry(IWallEntry entry)
        {
            lock (_lock)
            {
                if (!_isInitiallized) Init();
                return _knownUrls.Contains(entry.Url);
            }
        }
        internal static void AddNewEntry(IWallEntry entry)
        {
            lock (_lock)
            {
                if (!_isInitiallized) Init();
                _knownUrls.Add(entry.Url);
            }
        }
        internal static void Save()
        {
            lock (_lock)
            {
                if (!_isInitiallized) return;

                // save the List of known Urls so the Crawler may consider them next time  
                string file = $"{_dataPath.FullName}\\known.json";
                File.WriteAllText(file, JsonConvert.SerializeObject(
                    _knownUrls.Distinct().ToList(),
                    Formatting.Indented));
            }
        }


        internal static List<string> GetKnownUrls()
        {
            if (!_isInitiallized) Init();
            return _knownUrls;
        }

    }
}
