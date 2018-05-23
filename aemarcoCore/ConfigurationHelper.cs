using aemarcoCore.Properties;
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



        //wallpapersetter
        /// <summary>
        /// Used for WallpaperMode.AllowFill to determine how many % can be cut on 
        /// left and right side of the picture. Ranges 0 - 100%
        /// Example: 10 means that left side can be cutted 5 % and right side 5%
        /// </summary>
        public static int PercentLeftRightCutAllowed { get; set; } = Settings.Default.WallpaperPercentCutAllowedLeftRight;
        /// <summary>
        /// Used for WallpaperMode.AllowFill to determine how many % can be cut on 
        /// top and bottom side of the picture. Ranges 0 - 100%
        /// Example: 10 means that top side can be cutted 5 % and bottom side 5%
        /// </summary>
        public static int PercentTopBottomCutAllowed { get; set; } = Settings.Default.WallpaperPercentCutAllowedTopBottom;


        public static void Configure()
        {


        }












    }
}
