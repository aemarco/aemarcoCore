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

        #region fields

        //preperation
        private static bool _isInitiallized = false;

        //File holding known urls
        private static FileInfo _dataFile;


        //operation
        private static List<string> _knownUrls;
        private static object _lock = new object();

        #endregion

        #region preparation

        private static void Init()
        {
            _knownUrls = new List<string>();

            if (ConfigurationHelper.KnownUrlsFunc != null)
            {
                try
                {
                    _knownUrls = ConfigurationHelper.KnownUrlsFunc();
                    _isInitiallized = true;
                }
                catch
                {
                    ConfigurationHelper.KnownUrlsFunc = null;
                    Init();
                }
            }
            else if (ConfigurationHelper.KnownUrlsFile != null)
            {
                try
                {
                    _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(ConfigurationHelper.KnownUrlsFile));
                    _dataFile = new FileInfo(ConfigurationHelper.KnownUrlsFile);
                    _isInitiallized = true;
                }
                catch
                {
                    ConfigurationHelper.KnownUrlsFile = null;
                    Init();
                }
            }
            else
            {
                _dataFile = GetDataFileFileInfo();
                if (_dataFile.Exists)
                {
                    _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_dataFile.FullName));
                }
                _isInitiallized = true;
            }

        }
        private static FileInfo GetDataFileFileInfo()
        {
            DirectoryInfo dataPath;
            //get path for saving
            try
            {
                dataPath = new DirectoryInfo(Settings.Default.CrawlerData);
                if (!dataPath.Exists) dataPath.Create();
            }
            catch
            {
                dataPath = new DirectoryInfo($"{Environment.CurrentDirectory}\\JSON");
                if (!dataPath.Exists) dataPath.Create();

            }
            return new FileInfo($"{dataPath.FullName}\\known.json");
        }

        #endregion

        #region operation

        internal static bool IsKnownEntry(IWallEntry entry)
        {
            lock (_lock)
            {
                if (!_isInitiallized) Init();

                bool known = _knownUrls.Contains(entry.Url);
                if (!known)
                {
                    _knownUrls.Add(entry.Url);
                }
                return known;
            }
        }
        internal static List<string> GetKnownUrls()
        {
            lock (_lock)
            {
                if (!_isInitiallized) Init();
                return _knownUrls;
            }
        }

        #endregion

        #region conclusion

        internal static void Save()
        {
            if (!_isInitiallized) return;

            lock (_lock)
            {
                if (_dataFile != null)
                {
                    string json = JsonConvert.SerializeObject(
                        _knownUrls.Distinct().ToList(),
                        Formatting.Indented);
                    File.WriteAllText(_dataFile.FullName, json);
                }

                _isInitiallized = false;
                _dataFile = null;
                _knownUrls = null;
            }
        }

        #endregion

    }
}
