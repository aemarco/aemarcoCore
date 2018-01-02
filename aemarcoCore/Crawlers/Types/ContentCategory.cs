namespace aemarcoCore.Crawlers.Types
{
    internal class ContentCategory : IContentCategory
    {
        private string _mainCategory;
        private string _subCategory;

        internal ContentCategory()
        {
            _mainCategory = string.Empty;
            _subCategory = string.Empty;
        }

        internal void SetMainCategory(Category cat)
        {
            _mainCategory = cat.ToString();
        }
        internal void SetSubCategory(Category cat)
        {
            _subCategory = cat.ToString();
        }


        public string MainCategory { get { return _mainCategory; } }
        public string SubCategory { get { return _subCategory; } }


    }
}
