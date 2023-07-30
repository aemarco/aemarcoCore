

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace aemarco.Crawler.Wallpaper.Model;

public class ContentCategory
{
    internal ContentCategory(Category category, int? minAdult = null, int? maxAdult = null)
    {
        Category = category.ToString();

        var strings = Category.Split('_').ToList();
        MainCategory = strings[0];
        if (strings.Count > 1)
        {
            SubCategory = strings[1];
        }

        SuggestedMinAdultLevel = minAdult;
        SuggestedMaxAdultLevel = maxAdult;
    }

    [JsonIgnore]
    public string Category { get; }


    public string MainCategory { get; }
    public string SubCategory { get; } = string.Empty;
    public int? SuggestedMinAdultLevel { get; set; }
    public int? SuggestedMaxAdultLevel { get; set; }

}