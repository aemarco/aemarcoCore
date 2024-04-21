// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarco.Crawler.Person.Model;

public record ProfilePicture(string Url)
{
    public ProfilePicture(string url, int? suggestedMinAdultLevel, int? suggestedMaxAdultLevel)
        : this(url)
    {
        SuggestedMinAdultLevel = suggestedMinAdultLevel;
        SuggestedMaxAdultLevel = suggestedMaxAdultLevel;
    }

    public string Url { get; } = Url;
    public int? SuggestedMinAdultLevel { get; }
    public int? SuggestedMaxAdultLevel { get; }
}