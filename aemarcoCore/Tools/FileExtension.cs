using aemarcoCore.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace aemarcoCore.Tools
{
    internal static class FileExtension
    {
        internal static List<string> _crawlerExtensions;

        private static void Init()
        {
            _crawlerExtensions = new List<string>();
            foreach (var ext in Settings.Default.CrawlerFileExtensions.Split(','))
            {
                _crawlerExtensions.Add(ext.Replace(".", "").ToLower().Insert(0, "."));
            }
        }

        internal static string GetFileExtension(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            return Path.GetExtension(input).ToLower();
        }

        internal static bool IsCrawlerExtension(string extension)
        {
            if (extension == null)
            {
                return false;
            }

            if (_crawlerExtensions == null)
            {
                Init();
            }

            return _crawlerExtensions.Contains(extension);

        }







    }
}
