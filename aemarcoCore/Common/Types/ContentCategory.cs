

using Newtonsoft.Json;

namespace aemarcoCore.Common
{
    public class ContentCategory : IContentCategory
    {
        private Category _mainCategory;
        private Category _subCategory;

        public ContentCategory(Category mainCategory) : this(mainCategory, Category.None)
        {
        }
        public ContentCategory(Category mainCategory, Category subCategory)
        {
            _mainCategory = mainCategory;
            _subCategory = subCategory;
        }


        public string MainCategory
        { get { return _mainCategory.ToString(); } }
        public string SubCategory
        { get { return _subCategory.ToString(); } }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



    }
}
