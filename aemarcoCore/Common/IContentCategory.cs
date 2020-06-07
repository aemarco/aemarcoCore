using Newtonsoft.Json;

namespace aemarcoCore.Common
{
    public interface IContentCategory
    {
        [JsonIgnore]
        string Category { get; }

        string MainCategory { get; }
        string SubCategory { get; }

        int SuggestedMinAdultLevel { get; set; }
        int SuggestedMaxAdultLevel { get; set; }


        [JsonIgnore]
        string Json { get; }
    }
}
