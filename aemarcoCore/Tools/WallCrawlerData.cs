using aemarcoCore.Common;
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
        private static readonly object _lock = new object();

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
                var dataPath = new DirectoryInfo($"{Environment.CurrentDirectory}\\JSON");
                if (!dataPath.Exists) dataPath.Create();
                _dataFile = new FileInfo($"{dataPath.FullName}\\known.json");
                if (_dataFile.Exists)
                {
                    _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_dataFile.FullName));
                }
                _isInitiallized = true;
            }
        }


        #endregion

        #region operation

        /// <summary>
        /// check if the entry is already known
        /// after this check, the entry will be in the known list
        /// </summary>
        /// <param name="entry">entry to be checked</param>
        /// <returns>true if it´s knwon already</returns>
        internal static bool IsKnownEntry(IWallEntry entry)
        {
            lock (_lock)
            {
                if (!_isInitiallized) Init();

                var known = _knownUrls.Contains(entry.Url);
                if (!known)
                {
                    _knownUrls.Add(entry.Url);
                }
                return known;
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
                    var json = JsonConvert.SerializeObject(
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
