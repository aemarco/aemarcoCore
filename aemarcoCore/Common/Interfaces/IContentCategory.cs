using Newtonsoft.Json;

namespace aemarcoCore.Common
{
    public interface IContentCategory
    {
        string MainCategory { get; }
        string SubCategory { get; }

        [JsonIgnore]
        string JSON { get; }
    }
}
