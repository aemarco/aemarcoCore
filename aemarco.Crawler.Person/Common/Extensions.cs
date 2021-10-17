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
    }

}
