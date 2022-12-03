using System;
using System.Reflection;


namespace aemarco.Crawler.Person.Common
{
    public static class Extensions
    {
        public static PersonCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
        {
            return crawlerType.GetCustomAttribute<PersonCrawlerAttribute>();

        }


        public static bool IsAvailableCrawler(this Type crawlerType)
        {
            if (!crawlerType.IsSubclassOf(typeof(PersonCrawlerBase)))
                return false;

            if (crawlerType.Namespace is null || crawlerType.Namespace.EndsWith("Obsolete"))
                return false;


            return true;
        }
    }

}
