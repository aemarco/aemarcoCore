using aemarcoCore.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace aemarcoCore.Crawlers.Types
{
    internal class ContentCategory : IContentCategory
    {
        private string _category;
        private string _mainCategory;
        private string _subCategory;

        internal ContentCategory(Category category)
        {
            _category = category.ToString();

            List<string> strings = _category.Split('_').ToList();
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

        [JsonIgnore]
        public string Category
        { get { return _category; } }


        public string MainCategory
        { get { return _mainCategory; } }
        public string SubCategory
        { get { return _subCategory; } }


        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



    }
}
