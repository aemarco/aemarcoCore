using aemarcoCommons.Extensions.AttributeExtensions;
using System;


namespace aemarcoCommons.PersonCrawler.Common
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
