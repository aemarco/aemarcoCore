using aemarcoCore.Properties;
using System;
using System.Collections.Generic;

namespace aemarcoCore
{
    public static class ConfigurationHelper
    {

        //crawling
        public static Func<List<string>> KnownUrlsFunc { get; set; }
        public static string KnownUrlsFile { get; set; }







        public static int PercentLeftRightCutAllowed { get; set; } = 0;
        public static int PercentTopBottomCutAllowed { get; set; } = 0;


        public static void Configure()
        {
            PercentLeftRightCutAllowed = Settings.Default.WallpaperPercentCutAllowedLeftRight;
            PercentTopBottomCutAllowed = Settings.Default.WallpaperPercentCutAllowedTopBottom;

        }












    }
}
