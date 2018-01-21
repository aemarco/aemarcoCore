using aemarcoCore.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace aemarcoCore.Crawlers.Types
{
    internal class ContentCategory : IContentCategory
    {
        private string _mainCategory;
        private string _subCategory;

        internal ContentCategory(Category category)
        {
            List<string> strings = category.ToString().Split('_').ToList();
            _mainCategory = strings[0];

            if (strings.Count > 1)
            {
                _subCategory = strings[1];
            }
            else
            {
                _subCategory = string.Empty;
            }
        }


        public string MainCategory
        { get { return _mainCategory; } }
        public string SubCategory
        { get { return _subCategory; } }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



    }
}
