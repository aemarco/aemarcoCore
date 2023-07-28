namespace aemarco.Crawler.Common;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CrawlerAttribute : Attribute
{
    /// <summary>
    /// Specifies properties for the crawler
    /// </summary>
    /// <param name="friendlyName">friendly name for the crawler</param>
    /// <param name="priority">lower values have priority</param>
    public CrawlerAttribute(string friendlyName, int priority = int.MaxValue)
    {
        FriendlyName = friendlyName;
        Priority = priority;
    }

    public string FriendlyName { get; }
    public int Priority { get; }

}