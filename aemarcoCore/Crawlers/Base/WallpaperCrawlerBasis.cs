using aemarcoCore.Common;
using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class WallpaperCrawlerBasis : CrawlerBasis
    {
        #region fields

        //ctor
        private readonly bool _onlyNews;
        private readonly int _startPage;
        private readonly int _lastPage;
        private CancellationToken _cancellationToken;

        //handling
        private List<CrawlOffer> _catJobs;
        private int _entriesPerPage;

        #endregion

        #region ctor

        internal WallpaperCrawlerBasis(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
        {
            _startPage = startPage;
            _lastPage = lastPage;
            _cancellationToken = cancellationToken;
            _onlyNews = onlyNews;
        }


        #endregion

        #region props

        internal bool HasWorkingOffers
        {
            get
            {
                return _catJobs == null ||
                      _catJobs.Count > 0;
            }
        }

        #endregion

        #region Starting 

        internal void LimitAsPerFilterlist(List<string> searchedCategories)
        {
            //no Filtration leaves _offers null
            if (searchedCategories == null || searchedCategories.Count == 0)
            {
                return;
            }


            _catJobs = new List<CrawlOffer>();
            foreach (var offer in GetCrawlsOffers())
            {
                //job gets added if it matched any searchedCategories
                if (searchedCategories.Where(x => offer.MatchFilter(x)).FirstOrDefault() != null)
                {
                    _catJobs.Add(offer);
                }
            }
        }

        protected CrawlOffer CreateCrawlOffer(
            string categoryName, Uri categoryUri, IContentCategory category)
        {
            return new CrawlOffer(_startPage, _lastPage, _onlyNews,
                categoryName, categoryUri, category);
        }

        public bool Start()
        {
            //Crawling
            DoWork();

            if (_cancellationToken.IsCancellationRequested)
            {
                return false;

            }
            return true;
        }


        #endregion

        #region Events

        internal event EventHandler<ProgressChangedEventArgs> Progress;
        protected void OnProgress()
        {
            Progress?.Invoke(this, new ProgressChangedEventArgs(CalculateProgress(), null));
        }

        internal event EventHandler<IWallEntryEventArgs> KnownEntry;
        private void OnKnownEntry(IWallEntry entry)
        {
            KnownEntry?.Invoke(this, new IWallEntryEventArgs { Entry = entry });
        }

        internal event EventHandler<IWallEntryEventArgs> NewEntry;
        private void OnNewEntry(IWallEntry entry)
        {
            NewEntry?.Invoke(this, new IWallEntryEventArgs { Entry = entry });
        }

        #endregion

        #region Progress

        private int CalculateProgress()
        {
            int done = _catJobs.Sum(x => x.DoneEntries);
            int todo = _catJobs.Sum(x => x.TotalEntries);
            if (todo == 0)
            {
                return 0;
            }
            return Convert.ToInt32(100.0 * done / todo);
        }

        #endregion

        #region Crawling

        private void DoWork()
        {
            if (_catJobs == null)
            {
                _catJobs = GetCrawlsOffers();
            }

            foreach (var catJob in _catJobs)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                GetCategory(catJob);
            }
        }
        protected abstract List<CrawlOffer> GetCrawlsOffers();
        protected virtual void GetCategory(CrawlOffer catJob)
        {
            do
            {
                if (!GetPage(catJob))
                {
                    catJob.ReportEndReached();
                    break;
                }
            }
            while (catJob.ContainsPagesToDo && !_cancellationToken.IsCancellationRequested);
            //either no more work or cancellation
            OnProgress();
        }
        protected virtual bool GetPage(CrawlOffer catJob)
        {
            bool result = false;
            //Seite mit Wallpaperliste
            Uri pageUri = GetSiteUrlForCategory(catJob);
            var doc = GetDocument(pageUri);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(GetSearchStringGorEntryNodes());

            //non Valid Page
            if (nodes == null || nodes.Count == 0)
            {
                //invalid page breaks the loop
                return false;
            }

            if (nodes.Count > _entriesPerPage)
            {
                _entriesPerPage = nodes.Count;
                //set for all so progress can be calculated
                for (int i = 0; i < _catJobs.Count; i++)
                {
                    _catJobs[i].ReportNumberOfEntriesPerPage(_entriesPerPage);

                }
            }


            foreach (HtmlNode node in nodes)
            {
                if (_cancellationToken.IsCancellationRequested ||
                    catJob.ReachedMaximumStreak)
                {
                    catJob.ReportEndReached();
                    return true;
                }

                if (AddWallEntry(node, catJob))
                {
                    result = true;
                }
                OnProgress();
            }
            catJob.ReportPageDone();
            //valid Page contains minimum 1 valid Entry
            return result;
        }
        protected abstract Uri GetSiteUrlForCategory(CrawlOffer catJob);
        protected abstract string GetSearchStringGorEntryNodes();
        protected abstract IContentCategory GetContentCategory(string categoryName);
        protected abstract bool AddWallEntry(HtmlNode node, CrawlOffer catJob);
        protected void AddEntry(IWallEntry entry, CrawlOffer catJob)
        {
            //bekanntes Entry
            if (WallCrawlerData.IsKnownEntry(entry))
            {
                OnKnownEntry(entry);
                catJob.ReportEntryDone(true);
            }
            //neues Entry
            else
            {
                OnNewEntry(entry);
                catJob.ReportEntryDone(false);
            }
        }

        #endregion

    }
}
