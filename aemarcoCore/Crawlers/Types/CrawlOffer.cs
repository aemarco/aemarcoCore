using aemarcoCore.Common;
using System;

namespace aemarcoCore.Crawlers.Types
{
    internal class CrawlOffer
    {
        #region fields

        //Informations
        private readonly string _siteCategoryName;
        private readonly Uri _categoryUri;
        private IContentCategory _category;

        //paging

        private readonly int _startPage;
        private int _lastPage;
        private int _currentPage;
        private readonly bool _onlyNews;

        private int _numberOfEntriesPerPage;
        private int _numberOfEntriesDoneForCurrentPage;
        private int _numberOfEntriesKnownInSequence;
        private int _numberOfEntriesDone;
        private int _numberOfEntriesTotal;

        #endregion

        #region ctor

        public CrawlOffer(int startPage, int lastPage, bool onlyNews,
            string siteCategoryName, Uri categoryUri, IContentCategory category)
        {
            _startPage = startPage;
            _lastPage = lastPage;
            _currentPage = _startPage;
            _onlyNews = onlyNews;
            _siteCategoryName = siteCategoryName;
            _categoryUri = categoryUri;
            _category = category;
        }

        #endregion

        #region props

        //Informations
        public string SiteCategoryName
        {
            get { return _siteCategoryName; }
        }
        public Uri CategoryUri
        {
            get { return _categoryUri; }
        }
        public IContentCategory Category
        {
            get { return _category; }
        }

        //paging
        public bool ContainsPagesToDo
        {
            get { return _currentPage <= _lastPage; }
        }
        public bool ReachedMaximumStreak
        {
            get { return (_onlyNews && _numberOfEntriesKnownInSequence >= 10); }
        }
        public int CurrentPage
        {
            get { return _currentPage; }
        }
        public int DoneEntries
        {
            get { return _numberOfEntriesDone; }
        }
        public int TotalEntries
        {
            get { return _numberOfEntriesTotal; }
        }

        #endregion

        #region methods internal

        internal bool MatchFilter(string search)
        {
            //offers with no Category cant match any filter
            if (_category == null)
            {
                return false;
            }

            if (_category.Category == search)
            {
                return true;
            }

            return false;
        }

        internal void ReportNumberOfEntriesPerPage(int count)
        {
            int numberOfPages = _lastPage - _startPage + 1;
            _numberOfEntriesTotal = numberOfPages * count;
            _numberOfEntriesPerPage = count;
        }

        internal void ReportPageDone()
        {
            _currentPage++;
            _numberOfEntriesDoneForCurrentPage = 0;
        }

        internal void ReportEntryDone(bool known)
        {
            if (known)
            {
                _numberOfEntriesKnownInSequence++;
            }
            else
            {
                _numberOfEntriesKnownInSequence = 0;
            }


            _numberOfEntriesDoneForCurrentPage++;

            int countBefore = (_currentPage - _startPage) * _numberOfEntriesPerPage;
            _numberOfEntriesDone = countBefore + _numberOfEntriesDoneForCurrentPage;
        }

        internal void ReportEndReached()
        {
            _numberOfEntriesTotal = _numberOfEntriesDone;
            _lastPage = _currentPage - 1;
        }



        #endregion















    }
}
