using System;

namespace aemarco.Crawler.Core.Attributes
{
    public class PersonCrawlerAttribute : Attribute
    {
        public PersonCrawlerAttribute(string friendlyName, int priority, bool isEnabled = true)
        {
            FriendlyName = friendlyName;
            Priority = priority;
            IsEnabled = isEnabled;
        }

        public string FriendlyName { get; }
        public int Priority { get; }
        public bool IsEnabled { get; set; }
    }
}
