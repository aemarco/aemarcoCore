﻿using aemarcoCore.Common;
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

        private IProgress<int> _progress;
        private object _progressLock;
        private object _entryLock;

        private Dictionary<WallpaperCrawlerBasis, int> _crawlers;
        private WallCrawlerResult _result;
        private List<string> _filterCategories;

        private DirectoryInfo _reportPath;


        /// <summary>
        /// Used to crawl various Websites for Wallpapers
        /// </summary>
        /// <param name="cancellationToken">can be used for cancellation</param>
        /// <param name="progress">can be used to report progress (there is also a Event for this)</param>
        /// <param name="startPage">can be used in conjuction with lastPage to set first page where the crawler starts crawling</param>
        /// <param name="lastPage">can be used in conjuction with firstPage to set last page which the crawler crawls</param>
        public WallpaperCrawler(
            CancellationToken cancellationToken = default(CancellationToken),
            IProgress<int> progress = null,
            int startPage = 0,
            int lastPage = 0)
        {
            _progress = progress;
            _progressLock = new object();
            _entryLock = new object();

            _crawlers = new Dictionary<WallpaperCrawlerBasis, int>();
            _result = new WallCrawlerResult();
            _filterCategories = new List<string>();

            var crawlerTypes = System.Reflection.Assembly
                .GetAssembly(typeof(WallpaperCrawlerBasis))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
                .ToList();

            foreach (Type type in crawlerTypes)
            {
                var instance = (WallpaperCrawlerBasis)Activator.CreateInstance(type, startPage, lastPage, cancellationToken);
                _crawlers.Add(instance, 0);
            }
        }



        /// <summary>
        /// if set, IWallCrawlerResult will be saved there as json
        /// </summary>
        public string ReportPath
        {
            set { _reportPath = new DirectoryInfo(value); }
        }

        /// <summary>
        /// if set, the name will be set in the IWallCrawlerResult and
        /// used as prefix if a report is written
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
        public void AddCategoryFilter(Category category)
        {
            string cat = category.ToString();
            if (!_filterCategories.Contains(cat))
            {
                _filterCategories.Add(cat);
            }
        }


        /// <summary>
        /// Starts crawling, returns on completion
        /// </summary>
        /// <returns>IWallCrawlerResult</returns>
        public IWallCrawlerResult Start()
        {
            //reduce to crawlers which requires to run
            var crawlersToUse = new Dictionary<WallpaperCrawlerBasis, int>();
            foreach (var crawler in _crawlers.Keys)
            {
                crawler.LimitAsPerFilterlist(_filterCategories);
                if (crawler.HasWorkingOffers)
                {
                    crawler.Progress += Instance_OnProgress;
                    crawler.NewEntry += Instance_OnNewEntry;
                    crawler.KnownEntry += Instance_OnKnownEntry;
                    crawlersToUse.Add(crawler, 0);
                }
            }
            _crawlers = crawlersToUse;


            //Crawling
            List<Task> tasks = new List<Task>();
            foreach (var crawler in _crawlers.Keys)
            {
                var task = Task.Factory.StartNew(crawler.Start);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());


            //persist results for Deduplication
            WallCrawlerData.Save();
            //Writing Report
            WriteReport();


            OnCompleted();
            return _result;
        }
        /// <summary>
        /// Starts crawling by fire and forget
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

        private void WriteReport()
        {
            if (_reportPath != null)
            {
                try
                {
                    if (!_reportPath.Exists)
                    {
                        _reportPath.Create();
                    }

                    string prefix = string.Empty;
                    if (_result.ResultName.Length > 0)
                    {
                        prefix = $"{_result.ResultName}_";
                    }

                    File.WriteAllText
                        (
                        $"{_reportPath.FullName}\\{prefix}{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json",
                        _result.JSON
                        );
                }
                catch { }
            }
        }



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
                            ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                            if (syncer == null)
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
                        ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                        if (syncer == null)
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
                        ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                        if (syncer == null)
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
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
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

    }
}