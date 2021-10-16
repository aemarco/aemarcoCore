using System;
using aemarcoCommons.Extensions.AttributeExtensions;

namespace aemarco.Crawler.Person.Common
{
    public static class Extensions
    {
        public static PersonCrawlerAttribute ToCrawlerInfo(this Type crawlerType)
        {
            var attr = crawlerType.GetAttribute<PersonCrawlerAttribute>();
            return attr;
        }
    }

}
