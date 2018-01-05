using aemarcoCore.Crawlers.Types;
using aemarcoCore.Types;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public abstract class WallpaperCrawlerBasis
    {
        #region fields

        private CrawlerResult _result;

        private bool _onlyNews;
        private int _knownEntryStreak;
        private int _startPage;
        private int _lastPage;

        private IProgress<int> _progress;
        private CancellationToken _cancellationToken;
        private int _numberOfCategories;
        private int _numberOfCategoriesDone;
        private int _numberOfPages;
        private int _numberOfPagesDone;
        private int _numberOfEntries;
        private int _numberOfEntriesDone;
        #endregion

        #region ctor

        public WallpaperCrawlerBasis(
            string siteName,
            IProgress<int> progress,
            CancellationToken cancellationToken)
        {

            _result = new CrawlerResult(siteName);

            _onlyNews = true;
            _knownEntryStreak = 0;
            _startPage = 1;
            _lastPage = 10;

            _progress = progress;
            _cancellationToken = cancellationToken;
            _numberOfCategories = 0;
            _numberOfCategoriesDone = 0;
            _numberOfPages = _lastPage - _startPage + 1;
            _numberOfPagesDone = 0;
            _numberOfEntries = 0;
            _numberOfEntriesDone = 0;
        }
        public WallpaperCrawlerBasis(
            string siteName,
            int startPage,
            int lastPage,
            IProgress<int> progress,
            CancellationToken cancellationToken)
            : this(siteName, progress, cancellationToken)
        {
            _onlyNews = false;
            _startPage = startPage;
            _lastPage = lastPage;
            _numberOfPages = _lastPage - _startPage + 1;
        }

        #endregion

        #region Starting the thing


        public ICrawlerResult Start()
        {
            DoWork();
            CrawlerData.Save(_result);
            OnCompleted();
            return _result;
        }
        public void StartAsync()
        {
            Task.Factory.StartNew(Start);
        }
        public async Task<ICrawlerResult> StartAsyncTask()
        {
            return await Task.Factory.StartNew(Start);
        }

        #endregion

        #region Events

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<IWallEntry> KnownEntry;
        protected virtual void OnKnownEntry(IWallEntry entry)
        {
            if (KnownEntry != null)
            {
                foreach (Delegate d in KnownEntry.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, entry);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, entry });  // cleanup omitted
                    }
                }

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<IWallEntry> NewEntry;
        protected virtual void OnNewEntry(IWallEntry entry)
        {

            if (NewEntry != null)
            {
                foreach (Delegate d in NewEntry.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, entry);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, entry });  // cleanup omitted
                    }
                }

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<ICrawlerResult> Completed;
        protected virtual void OnCompleted()
        {
            if (Completed != null)
            {
                foreach (Delegate d in Completed.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(this, _result);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, _result });  // cleanup omitted
                    }
                }
            }
        }


        #endregion

        #region Progress

        protected void ReportNumberOfCategories(int count)
        {
            _numberOfCategories = count;

        }
        protected void ReportNumberOfEntries(int count)
        {
            if (count > _numberOfEntries)
            {
                _numberOfEntries = count;
            }
        }
        protected void ReportCategoryDone()
        {
            _numberOfCategoriesDone++;
            _numberOfPagesDone = 0;
            if (_progress != null)
            {
                _progress.Report(CalculateProgress());
            }
        }
        protected void ReportPageDone()
        {
            _numberOfPagesDone++;
            _numberOfEntriesDone = 0;

            if (_progress != null)
            {
                _progress.Report(CalculateProgress());
            }

        }
        protected void ReportEntryDone()
        {
            _numberOfEntriesDone++;
            if (_progress != null)
            {
                _progress.Report(CalculateProgress());
            }
        }
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

        protected virtual void DoWork()
        {
            Dictionary<string, string> categoriesToCrawl = GetCategoriesDict();

            ReportNumberOfCategories(categoriesToCrawl.Count);

            foreach (string cat in categoriesToCrawl.Keys)
            {
                if (!IShallGoAheadWithCategories())
                {
                    break;
                }
                GetCategory(cat, categoriesToCrawl[cat]);
            }
        }
        protected abstract Dictionary<string, string> GetCategoriesDict();
        protected virtual HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(url);
        }
        protected virtual void GetCategory(string categoryUrl, string categoryName)
        {
            int page = GetStartingPage();
            if (page == 0) page = 1;

            bool pageValid = true;
            do
            {
                pageValid = GetPage(GetSiteUrlForCategory(categoryUrl, page), categoryName);
                page++;


                //} while (page <= 1 && pageContainsNews);
            } while (pageValid && IShallGoAheadWithPages(page));
            ReportCategoryDone();
        }
        protected abstract string GetSiteUrlForCategory(string categoryUrl, int page);
        /// <summary>
        /// return true if page contains minimum 1 valid Entry
        /// </summary>        
        protected virtual bool GetPage(string pageUrl, string categoryName)
        {
            bool result = false;
            //Seite mit Wallpaperliste
            HtmlDocument doc = GetDocument(pageUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(GetSearchStringGorEntry());

            //non Valid Page
            if (nodes == null || nodes.Count == 0)
            {
                return result;
            }

            ReportNumberOfEntries(nodes.Count);

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
                ReportEntryDone();
            }

            //valid Page contains minimum 1 valid Entry
            ReportPageDone();
            return result;
        }

        protected abstract string GetSearchStringGorEntry();
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
            if (CrawlerData.IsKnownEntry(entry))
            {
                //Streak mitzählen
                _knownEntryStreak++;

                _result.AddKnownEntry(entry);
                OnKnownEntry(entry);
            }
            //neues Entry
            else
            {
                //Streak beenden
                _knownEntryStreak = 0;

                //url als bekannte Url merken
                CrawlerData.AddNewEntry(entry);

                _result.AddNewEntry(entry);
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
        protected virtual IContentCategory GetContentCategory(string categoryName)
        {
            return new ContentCategory(Category.None);
        }

        #endregion







    }
}
