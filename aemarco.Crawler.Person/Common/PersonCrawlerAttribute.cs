namespace aemarco.Crawler.Person.Common;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PersonCrawlerAttribute : Attribute
{
    public PersonCrawlerAttribute(string friendlyName, int priority)
    {
        FriendlyName = friendlyName;
        Priority = priority;
    }

    public string FriendlyName { get; }
    public int Priority { get; }
}