using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers
{
    public class PersonCrawler
    {
        #region fields

        readonly Dictionary<PersonCrawlerBase, int> _crawlers = new Dictionary<PersonCrawlerBase, int>();
        CancellationToken _cancellationToken;
        readonly IProgress<int> _progress;

        //settings
        readonly PersonCrawlerResult _result = new PersonCrawlerResult();
        private DirectoryInfo _reportPath = null;
        private readonly string _nameToCrawl;
        readonly List<string> _filterPersonSites = new List<string>();


        //events
        private readonly object _progressLock = new object();
        private readonly object _entryLock = new object();

        #endregion

        #region ctor

        /// <summary>
        /// Used to crawl various Websites for a Person
        /// </summary>
        /// <param name="cancellationToken">can be used for cancellation</param>
        /// <param name="progress">can be used to report progress (there is also a Event for this)</param>
        public PersonCrawler(string nameToCrawl,
            CancellationToken cancellationToken = default,
            IProgress<int> progress = null)
        {
            _nameToCrawl = nameToCrawl;
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
        /// Not using means all sites will be crawled
        /// Using means only sites added will be crawled
        /// </summary>
        /// <param name="personSite"></param>
        public void AddPersonSiteFilter(PersonSite personSite)
        {
            var site = personSite.ToString();
            if (!_filterPersonSites.Contains(site))
            {
                _filterPersonSites.Add(site);
            }

        }



        #endregion

        #region Working

        /// <summary>
        /// Starts crawling by fire and forget
        /// </summary>
        public void StartAsync()
        {
            Task.Factory.StartNew(Start);
        }

        /// <summary>
        /// Starts crawling, returns Task<IPersonCrawlerResult> on completion
        /// </summary>
        /// <returns>IPersonCrawlerResult</returns>
        public async Task<IPersonCrawlerResult> StartAsyncTask()
        {
            return await Task.Factory.StartNew(Start);
        }

        /// <summary>
        /// Starts crawling, returns on completion
        /// </summary>
        /// <returns>IPersonCrawlerResult</returns>
        public IPersonCrawlerResult Start()
        {
            PrepareCrawlerList();


            //start all crawlers
            var tasks = new List<Task<bool>>();
            foreach (var crawler in _crawlers.Keys)
            {
                var task = Task<bool>.Factory.StartNew(crawler.Start);
                tasks.Add(task);
            }

            try
            {
                //awaiting end of crawling
                Task.WaitAll(tasks.ToArray());
                if (!tasks.All(x => x.Result) == true)
                {
                    _result.HasBeenAborted = true;
                }
            }
            catch (Exception ex)
            {
                _result.Exception = ex;
            }


            //Writing Report
            WriteReport();

            OnCompleted();
            return _result;
        }

        private void PrepareCrawlerList()
        {
            //creates all available crawlers and adds them if applicable
            var crawlerTypes = System.Reflection.Assembly
                .GetAssembly(typeof(PersonCrawlerBase))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(PersonCrawlerBase)))
                .ToList();

            foreach (var type in crawlerTypes)
            {
                var instance = (PersonCrawlerBase)Activator.CreateInstance(type, _nameToCrawl, _cancellationToken);

                if (_filterPersonSites.Count > 0 && !_filterPersonSites.Contains(instance.PersonSite.ToString()))
                {
                    continue;
                }

                instance.Progress += Instance_OnProgress;
                instance.Entry += Instance_OnEntry;

                _crawlers.Add(instance, 0);
            }
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

                    var prefix = string.Empty;
                    if (!String.IsNullOrWhiteSpace(_result.ResultName))
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
                    var instance = (PersonCrawlerBase)sender;
                    _crawlers[instance] = e.ProgressPercentage;

                    var progress = _crawlers.Values.Sum() / _crawlers.Values.Count;


                    //falls IProgress
                    if (_progress != null)
                    {
                        _progress.Report(progress);
                    }

                    //falls Event
                    if (Progress != null)
                    {
                        foreach (var d in Progress.GetInvocationList())
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
        /// Delivers entries while crawling
        /// </summary>
        public event EventHandler<IPersonEntryEventArgs> Entry;
        private void Instance_OnEntry(object sender, IPersonEntryEventArgs e)
        {
            lock (_entryLock)
            {
                _result.AddNewEntry(e.Entry);

                if (Entry != null)
                {
                    foreach (var d in Entry.GetInvocationList())
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
        /// Delivers the IPersonCrawlerResult once crawling is completed
        /// </summary>
        public event EventHandler<IPersonCrawlerResultEventArgs> Completed;
        protected virtual void OnCompleted()
        {
            if (Completed != null)
            {
                foreach (var d in Completed.GetInvocationList())
                {
                    if (!(d.Target is ISynchronizeInvoke syncer))
                    {
                        d.DynamicInvoke(this, new IPersonCrawlerResultEventArgs { Result = _result });
                    }
                    else
                    {
                        syncer.BeginInvoke(d, new object[] { this, new IPersonCrawlerResultEventArgs { Result = _result } });  // cleanup omitted
                    }
                }
            }
        }

        #endregion

    }
}
