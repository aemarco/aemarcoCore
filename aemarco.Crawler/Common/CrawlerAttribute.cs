namespace aemarco.Crawler.Common;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CrawlerAttribute : Attribute
{
    /// <summary>
    /// Specifies properties for the crawler
    /// </summary>
    /// <param name="friendlyName">friendly name for the crawler</param>
    /// <param name="priority">lower values have priority</param>
    /// <param name="skipTesting"></param>
    public CrawlerAttribute(string friendlyName, int priority = int.MaxValue, bool skipTesting = false)
    {
        FriendlyName = friendlyName;
        Priority = priority;
        SkipTesting = skipTesting;
    }

    public string FriendlyName { get; }
    public int Priority { get; }
    public bool SkipTesting { get; }
}