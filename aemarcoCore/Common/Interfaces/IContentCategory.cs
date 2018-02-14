using Newtonsoft.Json;

namespace aemarcoCore.Common
{
    public interface IContentCategory
    {
        [JsonIgnore]
        string Category { get; }

        string MainCategory { get; }
        string SubCategory { get; }

        [JsonIgnore]
        string JSON { get; }
    }
}
