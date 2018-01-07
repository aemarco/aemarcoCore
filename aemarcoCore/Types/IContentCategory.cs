using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using aemarcoCore.Enums;

namespace aemarcoCore.Types
{
    public interface IContentCategory
    {
        [JsonConverter(typeof(StringEnumConverter))]
        Category MainCategory { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        Category SubCategory { get; }

    }
}
