using aemarco.Crawler.Core.Attributes;
using aemarcoCommons.Extensions.AttributeExtensions;
using System;

namespace aemarco.Crawler.Core.Extensions
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
