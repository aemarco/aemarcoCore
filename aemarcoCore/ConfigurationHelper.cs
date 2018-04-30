using aemarcoCore.Properties;
using aemarcoCore.Tools;
using System;
using System.Collections.Generic;

namespace aemarcoCore
{
    public static class ConfigurationHelper
    {
        public static int PercentLeftRightCutAllowed { get; set; } = 0;
        public static int PercentTopBottomCutAllowed { get; set; } = 0;


        public static void Configure()
        {
            PercentLeftRightCutAllowed = Settings.Default.WallpaperPercentCutAllowedLeftRight;
            PercentTopBottomCutAllowed = Settings.Default.WallpaperPercentCutAllowedTopBottom;

        }


        public static void SetUrlSourceFunc(Func<List<string>> urlSourceFunc)
        {
            WallCrawlerData.SetUrlSourceFunc(urlSourceFunc);
        }










    }
}
