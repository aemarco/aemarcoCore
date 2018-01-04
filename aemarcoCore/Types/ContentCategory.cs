namespace aemarcoCore.Types
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

        public Category MainCategory { get { return _mainCategory; } }
        public Category SubCategory { get { return _subCategory; } }


    }
}
