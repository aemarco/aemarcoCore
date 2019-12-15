using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using aemarcoCore.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public class WallpaperCrawler
    {
        #region fields

        internal Dictionary<WallpaperCrawlerBasis, int> _crawlers = new Dictionary<WallpaperCrawlerBasis, int>();

        //ctor
        private readonly bool _onlyNews;
        readonly int _startPage;
        readonly int _lastPage;
        readonly CancellationToken _cancellationToken;
        readonly IProgress<int> _progress;

        //settings
        private readonly WallCrawlerResult _result = new WallCrawlerResult();
        private DirectoryInfo _reportPath = null;
        readonly List<string> _filterCategories = new List<string>();
        readonly List<string> _filterSourceSites = new List<string>();

        //events
        private readonly object _progressLock = new object();
        private readonly object _entryLock = new object();

        #endregion

        #region ctor

        /// <summary>
        /// Used to crawl various Websites for Wallpapers
        /// </summary>
        /// <param name="cancellationToken">can be used for cancellation</param>
        /// <param name="progress">can be used to report progress (there is also a Event for this)</param>
        /// <param name="startPage">can be used in conjuction with lastPage to set first page where the crawler starts crawling</param>
        /// <param name="lastPage">can be used in conjuction with firstPage to set last page which the crawler crawls</param>
        public WallpaperCrawler(
            CancellationToken cancellationToken = default,
            IProgress<int> progress = null,
            int startPage = 0,
            int lastPage = 0)
        {
            if (startPage == 0 && lastPage == 0)
            {
                _onlyNews = true;
                _startPage = 1;
                _lastPage = 10;
            }
            else
            {
                _onlyNews = false;
                _startPage = startPage;
                _lastPage = lastPage;
            }


            if (_startPage < 1)
            {
                throw new ArgumentOutOfRangeException("Startpage needs to be 1 or higher");
            }
            if (_lastPage < _startPage)
            {
                throw new ArgumentOutOfRangeException("Lastpage can´t be smaller than Startpage");
            }


            _cancellationToken = cancellationToken;
            _progress = progress;
        }

        #endregion

        #region Settings

        /// <summary>
        /// if set, IWallCrawlerResult will be saved there as json
        /// </summary>
        public string ReportPath
        {
            set { _reportPath = new DirectoryInfo(value); }
        }

        /// <summary>
        /// if set, the name will be set in the IWallCrawlerResult and
        /// used as filename prefix if a file is requested by "ReportPath"
        /// </summary>
        public string ReportName
        {
            set { _result.ResultName = value; }
        }

        /// <summary>
        /// Used to filter which results beeing of interest to crawl
        /// Not using this means everything will be crawled
        /// Using this means only added categories will be crawled
        /// </summary>
        public void AddCategoryFilter(Common.Category category)
        {
            string cat = category.ToString();
            if (!_filterCategories.Contains(cat))
            {
                _filterCategories.Add(cat);
            }
        }

        /// <summary>
        /// Used to filter which sites to limit to
        /// Not using means all sites will be crawled
        /// Using menas only sites added will be crawled
        /// </summary>
        /// <param name="sourceSite"></param>
        public void AddSourceSiteFilter(SourceSite sourceSite)
        {
            string site = sourceSite.ToString();
            if (!_filterSourceSites.Contains(site))
            {
                _filterSourceSites.Add(site);
            }
        }

        #endregion

        #region Working

        /// <summary>
        /// Starts crawling by fire and forget
        /// events may be used for data and completion
        /// </summary>
        public void StartAsync()
        {
            Task.Factory.StartNew(Start);
        }

        /// <summary>
        /// Starts crawling, returns Task<IWallCrawlerResult> on completion
        /// </summary>
        /// <returns>IWallCrawlerResult</returns>
        public async Task<IWallCrawlerResult> StartAsyncTask()
        {
            return await Task.Factory.StartNew(Start);
        }

        /// <summary>
        /// Starts crawling, returns IWallCrawlerResult on completion
        /// </summary>
        /// <returns>IWallCrawlerResult</returns>
        public IWallCrawlerResult Start()
        {
            PrepareCrawlerList();

            _result.NumberOfCrawlersInvolved = _crawlers.Count;
            _result.SitesFilter = _filterSourceSites.ToList();
            _result.CategoryFilter = _filterCategories.ToList();


            try
            {
                //start all crawlers
                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (var crawler in _crawlers.Keys)
                {
                    _result.CrawlersInvolved.Add(crawler.GetType().Name);

                    var task = Task<bool>.Factory.StartNew(() => crawler.Start(this));
                    tasks.Add(task);
                }

                //awaiting end of crawling
                Task.WaitAll(tasks.ToArray());
                if (!tasks.All(x => x.Result) == true)
                {
                    _result.HasBeenAborted = true;
                }

            }
            catch (Exception ex) //since crawlers should not throw, this should never happen ?!
            {
                _result.Exceptions.Add(ex);
            }


            //persist results for Deduplication
            WallCrawlerData.Save();
            //Writing Report
            WriteReport();


            OnCompleted();
            return _result;
        }

        internal void PrepareCrawlerList()
        {
            //creates all available crawlers and adds them if applicable
            var crawlerTypes = System.Reflection.Assembly
                .GetAssembly(typeof(WallpaperCrawlerBasis))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
                .ToList();

            foreach (Type type in crawlerTypes)
            {
                var instance = (WallpaperCrawlerBasis)Activator.CreateInstance(type, _startPage, _lastPage, _onlyNews, _cancellationToken);

                //remove disabled crawlers
                if (instance.SourceSite.IsDisabled())
                {
                    continue;
                }

                //filter down to desired sites if site filter beeing used
                if (_filterSourceSites.Any() && !_filterSourceSites.Contains(instance.SourceSite.ToString()))
                {
                    continue;
                }

                //filter down to crawlers supporting any searched category if any searched category
                if (_filterCategories.Any() && !instance.SourceSite.SupportsAny(_filterCategories))
                {
                    continue;
                }

                try
                {
                    //reduce crawler to desired categories
                    instance.LimitAsPerFilterlist(_filterCategories);
                    //crawler confirms found category
                    if (instance.HasWorkingOffers)
                    {
                        //atach events
                        instance.Progress += Instance_OnProgress;
                        instance.NewEntry += Instance_OnNewEntry;
                        instance.KnownEntry += Instance_OnKnownEntry;

                        _crawlers.Add(instance, 0);
                    }
                }
                catch (Exception ex)
                {
                    _result.Exceptions.Add(ex);
                }
            }
        }

        internal void AddException(Exception ex)
        {
            _result.Exceptions.Add(ex);
        }

        private void WriteReport()
        {
            if (_reportPath != null)
            {
                try
                {
                    if (!_reportPath.Exists) _reportPath.Create();

                    string filename = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json";
                    if (!String.IsNullOrWhiteSpace(_result.ResultName))
                        filename.Insert(0, $"{_result.ResultName}_");


                    File.WriteAllText(Path.Combine(_reportPath.FullName, filename), _result.JSON);
                }
                catch { }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Delivers the progress of crawling 0...100
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> Progress;
        private void Instance_OnProgress(object sender, ProgressChangedEventArgs e)
        {
            //muss nur gerechnet werden wenn Event oder IProgress genutzt wird
            if (_progress != null || Progress != null)
            {
                lock (_progressLock)
                {
                    WallpaperCrawlerBasis instance = (WallpaperCrawlerBasis)sender;
                    _crawlers[instance] = e.ProgressPercentage;

                    int progress = _crawlers.Values.Sum() / _crawlers.Values.Count;

                    //falls IProgress
                    if (_progress != null)
                    {
                        _progress.Report(progress);
                    }

                    //falls Event
                    if (Progress != null)
                    {
                        foreach (Delegate d in Progress.GetInvocationList())
                        {
                            if (!(d.Target is ISynchronizeInvoke syncer))
                            {
                                d.DynamicInvoke(this, new ProgressChangedEventArgs(progress, null));
                            }
                            else
                            {
                                syncer.BeginInvoke(d, new object[] { this, new ProgressChangedEventArgs(progress, null) });  // cleanup omitted
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Delivers entries which are already known by the crawler
        /// </summary>
        public event EventHandler<IWallEntryEventArgs> KnownEntry;
        private void Instance_OnKnownEntry(object sender, IWallEntryEventArgs e)
        {
            lock (_entryLock)
            {
                _result.AddKnownEntry(e.Entry);

                if (KnownEntry != null)
                {
                    foreach (Delegate d in KnownEntry.GetInvocationList())
                    {
                        if (!(d.Target is ISynchronizeInvoke syncer))
                        {
                            d.DynamicInvoke(this, e);
                        }
                        else
                        {
                            syncer.BeginInvoke(d, new object[] { this, e });  // cleanup omitted
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delivers entries which are new to the crawler
        /// </summary>
        public event EventHandler<IWallEntryEventArgs> NewEntry;
        private void Instance_OnNewEntry(object sender, IWallEntryEventArgs e)
        {
            lock (_entryLock)
            {
                _result.AddNewEntry(e.Entry);


                if (NewEntry != null)
                {
                    foreach (Delegate d in NewEntry.GetInvocationList())
                    {
                        if (!(d.Target is ISynchronizeInvoke syncer))
                        {
                            d.DynamicInvoke(this, e);
                        }
                        else
                        {
                            syncer.BeginInvoke(d, new object[] { this, e });  // cleanup omitted
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Delivers the IWallCrawlerResult once crawling is completed
        /// </summary>
        public event EventHandler<IWallCrawlerResultEventArgs> Completed;
        protected virtual void OnCompleted()
        {
            if (Completed != null)
            {
                foreach (Delegate d in Completed.GetInvocationList())
                {
                    if (!(d.Target is ISynchronizeInvoke syncer))
                    {
                        d.DynamicInvoke(this, new IWallCrawlerResultEventArgs { Result = _result });
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, new IWallCrawlerResultEventArgs { Result = _result } });  // cleanup omitted
                    }
                }
            }
        }

        #endregion
    }
}
