using aemarcoCore.Common;
using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            CancellationToken cancellationToken)
        {
            _knownEntryStreak = 0;


            _onlyNews = true;
            _startPage = 1;
            _lastPage = 10;
            if (startPage != 0 && lastPage != 0)
            {
                _onlyNews = false;
                _startPage = startPage;
                _lastPage = lastPage;
            }
            _cancellationToken = cancellationToken;



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

        #endregion

        #region Starting 


        public void LimitAsPerFilterlist(List<string> categorys)
        {
            //no Filtration leaves _offers null
            if (categorys == null || categorys.Count == 0)
            {
                return;
            }


            //get offers
            if (_offers == null)
            {
                _offers = GetCrawlsOffers();
            }

            //new list with offers which will be used
            var used = new List<CrawlOffer>();

            foreach (var offer in _offers)
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

                if (categorys.Where(x => filterString.StartsWith(x)).FirstOrDefault() != null &&
                    !used.Contains(offer))
                {
                    used.Add(offer);
                }
            }
            _offers = used;
        }



        public void Start()
        {
            //Crawling
            DoWork();
        }


        #endregion

        #region Events


        public event EventHandler<ProgressChangedEventArgs> Progress;
        protected virtual void OnProgress()
        {
            Progress?.Invoke(this, new ProgressChangedEventArgs(CalculateProgress(), null));
        }


        public event EventHandler<IWallEntryEventArgs> KnownEntry;
        protected virtual void OnKnownEntry(IWallEntry entry)
        {
            if (KnownEntry != null)
            {
                foreach (Delegate d in KnownEntry.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, new IWallEntryEventArgs { Entry = entry });
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, new IWallEntryEventArgs { Entry = entry } });  // cleanup omitted
                    }
                }

            }
        }


        public event EventHandler<IWallEntryEventArgs> NewEntry;
        protected virtual void OnNewEntry(IWallEntry entry)
        {

            if (NewEntry != null)
            {
                foreach (Delegate d in NewEntry.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, new IWallEntryEventArgs { Entry = entry });
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, new IWallEntryEventArgs { Entry = entry } });  // cleanup omitted
                    }
                }

            }
        }

        #endregion

        #region Progress

        private int CalculateProgress()
        {
            int a = _numberOfCategoriesDone * _numberOfPages * _numberOfEntries +
                    _numberOfPagesDone * _numberOfEntries +
                    _numberOfEntriesDone;
            int b = _numberOfCategories * _numberOfPages * _numberOfEntries;
            double result = (100.0 * a) / (b);
            return (int)result;
        }

        #endregion

        #region Cancellation

        protected bool IShallGoAheadWithCategories()
        {
            return !_cancellationToken.IsCancellationRequested;
        }
        protected int GetStartingPage()
        {
            _knownEntryStreak = 0;
            return _startPage;
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

        private void DoWork()
        {
            if (_offers == null)
            {
                _offers = GetCrawlsOffers();
            }

            _numberOfCategories = _offers.Count;
            foreach (var offer in _offers)
            {
                if (!IShallGoAheadWithCategories())
                {
                    break;
                }
                GetCategory(offer.Url, offer.Name);
                _numberOfCategoriesDone++;
                _numberOfPagesDone = 0;

                OnProgress();
            }
        }
        protected abstract Dictionary<string, string> GetCategoriesDict();
        protected abstract List<CrawlOffer> GetCrawlsOffers();
        protected HtmlDocument GetDocument(string url, int retry = 0)
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
        private void GetCategory(string categoryUrl, string categoryName)
        {
            int page = GetStartingPage();
            if (page == 0) page = 1;

            bool pageValid = true;
            do
            {
                pageValid = GetPage(GetSiteUrlForCategory(categoryUrl, page), categoryName);
                _numberOfPagesDone++;
                _numberOfEntriesDone = 0;
                OnProgress();
                page++;
            } while (pageValid && IShallGoAheadWithPages(page));
        }
        protected abstract string GetSiteUrlForCategory(string categoryUrl, int page);
        /// <summary>
        /// return true if page contains minimum 1 valid Entry
        /// </summary>        
        private bool GetPage(string pageUrl, string categoryName)
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
        /// <summary>
        /// returns true if Entry is valid
        /// </summary>
        protected virtual bool AddWallEntry(HtmlNode node, string categoryName)
        {
            return false;
        }

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

        protected virtual string GetFileName(string url, string prefix)
        {
            return Path.GetFileNameWithoutExtension($"{prefix}{url?.Substring(url.LastIndexOf("/") + 1)}");
        }
        protected virtual List<string> GetTagsFromTagString(string tagString)
        {
            //z.B. "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"
            List<string> result = new List<string>();

            if (String.IsNullOrEmpty(tagString))
            {
                return result;
            }
            else
            {
                string[] tags = tagString.Split(',');
                foreach (string tag in tags)
                {
                    //z.B. "flowerdress"
                    string entry = tag.Trim();
                    if (entry.Length > 0)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }
        protected virtual List<string> GetTagsFromNodes(HtmlNodeCollection nodes)
        {

            List<string> result = new List<string>();
            if (nodes == null)
            {
                return result;
            }

            foreach (var node in nodes)
            {
                string entry = node.InnerText.Trim();
                if (entry.Length > 0)
                {
                    result.Add(entry);
                }
            }

            return result;
        }
        protected virtual string GetThumbnailUrlRelative(string url, HtmlNode node)
        {
            HtmlNode imageNode = node.SelectSingleNode("./img");
            return $"{url}{imageNode?.Attributes["src"]?.Value?.Substring(1)}";
        }
        protected virtual string GetThumbnailUrlAbsolute(HtmlNode node)
        {
            HtmlNode imageNode = node.SelectSingleNode("./img");
            return imageNode?.Attributes["src"]?.Value;
        }
        protected abstract IContentCategory GetContentCategory(string categoryName);


        #endregion







    }
}
