using aemarcoCore.Tools;
using System;
using System.Collections.Generic;

namespace aemarcoCore
{
    public static class ConfigurationHelper
    {

        public static void Configure()
        {

        }


        public static void SetUrlSourceFunc(Func<List<string>> urlSourceFunc)
        {
            WallCrawlerData.SetUrlSourceFunc(urlSourceFunc);
        }










    }
}
