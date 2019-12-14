using System;
using System.Collections.Generic;

namespace aemarcoCore
{
    public static class ConfigurationHelper
    {

        //crawling
        /// <summary>
        /// Func which will be called to get a List of known Urls for crawling.
        /// Enjoys 1. Priority
        /// </summary>
        public static Func<List<string>> KnownUrlsFunc { get; set; }
        /// <summary>
        /// A path to a file which will be used to load and save known Urls.
        /// Its formatted in json List<string>
        /// Enjoys 2. Priority
        /// </summary>
        public static string KnownUrlsFile { get; set; }

        public static string AbyssAPI_Key { get; set; }


    }
}
