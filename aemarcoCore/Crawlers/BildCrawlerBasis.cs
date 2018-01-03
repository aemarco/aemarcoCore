using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public abstract class BildCrawlerBasis
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

        public BildCrawlerBasis(
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
        public BildCrawlerBasis(
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

        protected abstract void DoWork();
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


        protected bool IsValidEntry(IWallEntry wallEntry)
        {
            //TODO: Some more validation

            if (
                wallEntry == null || //Entry darf nicht null sein
                String.IsNullOrEmpty(wallEntry.Url) || //Entry muss Url haben
                !FileExtension.IsCrawlerExtension(wallEntry.Extension) //Extension muss erlaubt sein
                )
            {
                return false;
            }

            return true;
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

        protected HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(url);
        }
        protected string GetEntryCategory(string url, string categoryName)
        {
            string search = $"{url}---{categoryName}";

            switch (search)
            {
                case "http://ftopx.com/---Girls & Cars":
                    {
                        return "Autos";
                    }
                case "http://ftopx.com/---Girls & Bikes":
                    {
                        return "Motorräder";
                    }
                case "http://ftopx.com/---Fantasy Girls":
                case "http://ftopx.com/---3D & Vector Girls":
                    {
                        return "Fantasy";
                    }
                case "http://ftopx.com/---Celebrity Fakes":
                    {
                        return "Celebrityfakes";
                    }
                case "http://ftopx.com/---Fetish Girls":
                    {
                        return "Fetischgirls";
                    }
                default:
                    {
                        return "Girls";
                    }
            }
        }

        internal ContentCategory GetEntryContentCategory(string siteName, string categoryName)
        {
            switch (siteName)
            {
                case "ftopx":
                    {
                        return GetFtopCategory(categoryName);
                    }
                case "adultwalls":
                    {
                        return GetAdultwallsCategory(categoryName);
                    }
                case "erowall":
                    {
                        return getErowallCategory(categoryName);
                    }
                case "zoomgirls":
                    {
                        ContentCategory result = new ContentCategory();
                        result.SetMainCategory(Category.Girls);
                        return result;
                    }
                case "pornomass":
                case "gifpornomass":
                    {
                        ContentCategory result = new ContentCategory();
                        result.SetMainCategory(Category.Girls);
                        result.SetSubCategory(Category.Hardcore);
                        return result;
                    }
                default:
                    {
                        ContentCategory result = new ContentCategory();
                        return result;
                    }
            }
        }



        private ContentCategory GetAdultwallsCategory(string categoryName)
        {
            ContentCategory result = new ContentCategory();
            result.SetMainCategory(Category.Girls);
            switch (categoryName)
            {
                case "Lingerie Models":
                    {
                        result.SetSubCategory(Category.Lingerie);
                        break;
                    }
            }
            return result;
        }

        private ContentCategory GetFtopCategory(string categoryName)
        {
            ContentCategory result = new ContentCategory();
            result.SetMainCategory(Category.Girls);

            switch (categoryName)
            {
                case "Celebrities":
                    {
                        result.SetSubCategory(Category.Celebrities);
                        break;
                    }
                case "Girls & Beaches":
                    {
                        result.SetSubCategory(Category.Beaches);
                        break;
                    }
                case "Girls & Cars":
                    {
                        result.SetSubCategory(Category.Cars);
                        break;
                    }
                case "Girls & Bikes":
                    {
                        result.SetSubCategory(Category.Bikes);
                        break;
                    }
                case "Lingerie Girls":
                    {
                        result.SetSubCategory(Category.Lingerie);
                        break;
                    }
                case "Asian Girls":
                    {
                        result.SetSubCategory(Category.Asian);
                        break;
                    }
                case "Holidays":
                    {
                        result.SetSubCategory(Category.Holidays);
                        break;
                    }
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    {
                        result.SetSubCategory(Category.Fantasy);
                        break;
                    }
                case "Celebrity Fakes":
                    {
                        result.SetSubCategory(Category.CelebrityFakes);
                        break;
                    }
                case "Fetish Girls":
                    {
                        result.SetSubCategory(Category.Fetish);
                        break;
                    }
            }
            return result;
        }

        private ContentCategory getErowallCategory(string categoryName)
        {
            ContentCategory result = new ContentCategory();
            result.SetMainCategory(Category.Girls);

            switch (categoryName)
            {

                case "Blowjob":
                    {
                        result.SetSubCategory(Category.Blowjob);
                        break;
                    }
                case "Lesbians":
                    {
                        result.SetSubCategory(Category.Lesbians);
                        break;
                    }
                case "Lingerie":
                    {
                        result.SetSubCategory(Category.Lingerie);
                        break;
                    }
                case "Beach":
                    {
                        result.SetSubCategory(Category.Beaches);
                        break;
                    }
                case "Asian":
                    {
                        result.SetSubCategory(Category.Asian);
                        break;
                    }
                case "Anime":
                    {
                        result.SetSubCategory(Category.Fantasy);
                        break;
                    }

            }

            return result;
        }

        #endregion



    }
}
