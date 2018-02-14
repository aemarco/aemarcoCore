using aemarcoCore.Common;
using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class WallpaperCrawlerBasis
    {
        #region fields

        private bool _onlyNews;
        private int _knownEntryStreak;
        private int _startPage;
        private int _lastPage;

        private CancellationToken _cancellationToken;
        private int _numberOfCategories;
        private int _numberOfCategoriesDone;
        private int _numberOfPages;
        private int _numberOfPagesDone;
        private int _numberOfEntries;
        private int _numberOfEntriesDone;

        private List<CrawlOffer> _offers;

        #endregion

        #region ctor

        internal WallpaperCrawlerBasis(
            int startPage,
            int lastPage,
            CancellationToken cancellationToken,
            bool onlyNews)
        {

            _knownEntryStreak = 0;

            _startPage = startPage;
            _lastPage = lastPage;
            _cancellationToken = cancellationToken;
            _onlyNews = onlyNews;


            _numberOfCategories = 0;
            _numberOfCategoriesDone = 0;
            _numberOfPages = _lastPage - _startPage + 1;
            _numberOfPagesDone = 0;
            _numberOfEntries = 0;
            _numberOfEntriesDone = 0;
        }


        #endregion

        #region props

        internal bool HasWorkingOffers
        {
            get
            {
                return _offers == null ||
                      _offers.Count > 0;
            }
        }

        protected int StartingPage
        {
            get
            {
                return _startPage;
            }
        }

        protected int NumberOfCategoriesDone
        {
            get { return _numberOfCategoriesDone; }
            set
            {
                _numberOfCategoriesDone = value;
                _knownEntryStreak = 0;
                NumberOfPagesDone = 0;
            }
        }

        protected int NumberOfPagesDone
        {
            get { return _numberOfPagesDone; }
            set
            {
                _numberOfPagesDone = value;
                _numberOfEntriesDone = 0;
                OnProgress();
            }
        }


        #endregion

        #region Starting 


        internal void LimitAsPerFilterlist(List<string> categories)
        {
            //no Filtration leaves _offers null
            if (categories == null || categories.Count == 0)
            {
                return;
            }
            

            _offers = new List<CrawlOffer>();
            foreach (var offer in GetCrawlsOffers())
            {
                //offers with no mainCategory cant match any filter
                if (String.IsNullOrEmpty(offer.MainCategory))
                {
                    continue;
                }

                string filterString = offer.MainCategory;
                if (!String.IsNullOrEmpty(offer.SubCategory))
                {
                    filterString += $"_{offer.SubCategory}";
                }

                if (categories.Where(x => filterString.StartsWith(x)).FirstOrDefault() != null &&
                    _offers.Contains(offer))
                {
                    _offers.Add(offer);
                }
            }
        }



        public void Start()
        {
            if (_offers == null)
            {
                _offers = GetCrawlsOffers();
            }


            //Crawling
            DoWork();
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
            int done = _numberOfCategoriesDone * _numberOfPages * _numberOfEntries +
                    _numberOfPagesDone * _numberOfEntries +
                    _numberOfEntriesDone;
            int todo = _numberOfCategories * _numberOfPages * _numberOfEntries;
            double result = 100.0 * done / todo; 
            if (double.IsNaN(result))
            {
                result = 0.0;
            }
            return Convert.ToInt32(result);
        }

        #endregion

        #region Cancellation

        protected bool IShallGoAheadWithCategories()
        {
            return !_cancellationToken.IsCancellationRequested;
        }

        protected bool IShallGoAheadWithPages(int currPage)
        {
            return !_cancellationToken.IsCancellationRequested &&
                currPage <= _lastPage &&
                (!_onlyNews || _knownEntryStreak < 10);
        }
        protected bool IShallGoAheadWithEntries()
        {
            return !_cancellationToken.IsCancellationRequested &&
                (!_onlyNews || _knownEntryStreak < 10);
        }

        #endregion

        #region Crawling

        protected virtual void DoWork()
        {
            
            _numberOfCategories = _offers.Count;

            foreach (var offer in _offers)
            {
                if (!IShallGoAheadWithCategories())
                {
                    break;
                }
                GetCategory(offer);
                NumberOfCategoriesDone++;
            }
        }
        protected abstract List<CrawlOffer> GetCrawlsOffers();
        protected virtual HtmlDocument GetDocument(string url, int retry = 0)
        {
            HtmlWeb web = new HtmlWeb();
            try
            {
                return web.Load(url);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.TrustFailure)
                {
                    return web.Load(url.Replace("https", "http"));
                }
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    if (retry >= 5)
                    {
                        throw;
                    }
                    return GetDocument(url, retry++);
                }
                throw;
            }
        }
        protected virtual void GetCategory(CrawlOffer offer)
        {
            int page = StartingPage;
            NumberOfPagesDone = 0;

            bool pageValid = true;
            do
            {
                if (GetPage(GetSiteUrlForCategory(offer.Url, page), offer.Name))
                {
                    NumberOfPagesDone++;
                    page++;
                }
                else
                {
                    pageValid = false;
                }
            } while (pageValid && IShallGoAheadWithPages(page));
        }
        protected abstract string GetSiteUrlForCategory(string categoryUrl, int page);
        protected virtual bool GetPage(string pageUrl, string categoryName)
        {
            bool result = false;
            //Seite mit Wallpaperliste
            HtmlDocument doc = GetDocument(pageUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(GetSearchStringGorEntryNodes());

            //non Valid Page
            if (nodes == null || nodes.Count == 0)
            {
                return result;
            }

            if (nodes.Count > _numberOfEntries)
            {
                _numberOfEntries = nodes.Count;
            }

            foreach (HtmlNode node in nodes)
            {
                if (!IShallGoAheadWithEntries())
                {
                    result = false;
                    break;
                }

                if (AddWallEntry(node, categoryName))
                {
                    result = true;
                }
                _numberOfEntriesDone++;
                OnProgress();
            }

            //valid Page contains minimum 1 valid Entry
            return result;
        }
        protected abstract string GetSearchStringGorEntryNodes();
        protected abstract IContentCategory GetContentCategory(string categoryName);
        protected abstract bool AddWallEntry(HtmlNode node, string categoryName);
        protected void AddEntry(IWallEntry entry)
        {
            //bekanntes Entry
            if (WallCrawlerData.IsKnownEntry(entry))
            {
                //Streak mitzählen
                _knownEntryStreak++;

                OnKnownEntry(entry);
            }
            //neues Entry
            else
            {
                //Streak beenden
                _knownEntryStreak = 0;

                //url als bekannte Url merken
                WallCrawlerData.AddNewEntry(entry);

                OnNewEntry(entry);
            }
        }

        #endregion





    }
}
