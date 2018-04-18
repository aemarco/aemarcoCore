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

        private static KnownUrlSource _knownUrlSource = KnownUrlSource.FileFromSettings;
        //FileFromSettings
        private static FileInfo _dataFile;
        //ForeignFunc
        private static Func<List<string>> _urlSourceFunc;


        private static List<string> _knownUrls;
        private static object _lock = new object();


        #region preparation


        private static void Init()
        {
            _knownUrls = new List<string>();

            switch (_knownUrlSource)
            {
                case KnownUrlSource.FileFromSettings:
                    _dataFile = GetDataFileFileInfo();
                    if (_dataFile.Exists)
                    {
                        _knownUrls = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_dataFile.FullName));
                    }
                    break;

                case KnownUrlSource.ForeignFunc:
                    _knownUrls = _urlSourceFunc();
                    break;

                default:
                    throw new ArgumentException("KnownUrlSource not implemented");
            }
            _isInitiallized = true;
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


        internal static void SetUrlSourceFunc(Func<List<string>> urlSourceFunc)
        {
            _urlSourceFunc = urlSourceFunc;
            _knownUrlSource = KnownUrlSource.ForeignFunc;
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
            if (!_isInitiallized) Init();
            return _knownUrls;
        }

        #endregion

        #region conclusion

        internal static void Save()
        {
            lock (_lock)
            {
                if (!_isInitiallized) return;


                switch (_knownUrlSource)
                {
                    case KnownUrlSource.FileFromSettings:
                        SaveKnownUrlsToFile();
                        break;
                    case KnownUrlSource.ForeignFunc:
                        //foreign source handles known Urls by itself
                        break;
                    default:
                        throw new ArgumentException("KnownUrlSource not implemented");
                }
                Conclude();
            }
        }

        private static void SaveKnownUrlsToFile()
        {
            string json = JsonConvert.SerializeObject(
                    _knownUrls.Distinct().ToList(),
                    Formatting.Indented);
            File.WriteAllText(_dataFile.FullName, json);

        }

        internal static void Conclude()
        {
            _isInitiallized = false;
            _dataFile = null;
            _knownUrls = null;
        }

        #endregion

    }
}
