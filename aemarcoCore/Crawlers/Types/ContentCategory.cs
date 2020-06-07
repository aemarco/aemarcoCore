using aemarcoCore.Common;
using Newtonsoft.Json;
using System.Linq;


namespace aemarcoCore.Crawlers.Types
{
    internal class ContentCategory : IContentCategory
    {
        internal ContentCategory(Category category, int minAdult = -1, int maxAdult = -1)
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
        public int SuggestedMinAdultLevel { get; set; }
        public int SuggestedMaxAdultLevel { get; set; }


        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



    }
}
