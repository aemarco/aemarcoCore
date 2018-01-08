using aemarcoCore.Common;
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
    public class WallpaperCrawlerAll
    {

        private Dictionary<WallpaperCrawlerBasis, int> _crawlers;
        private IProgress<int> _progress;
        WallCrawlerResult _result;
        DirectoryInfo _reportPath;
        private object _progressLock;
        private object _entryLock;

        public WallpaperCrawlerAll(
            IProgress<int> progress = null,
            CancellationToken cancellationToken = default(CancellationToken),
            DirectoryInfo reportpath = null)
        {
            _crawlers = new Dictionary<WallpaperCrawlerBasis, int>();

            _progress = progress;
            _result = new WallCrawlerResult("All Crawlers");
            _reportPath = reportpath;
            _progressLock = new object();
            _entryLock = new object();

            var crawlerTypes = System.Reflection.Assembly
                .GetAssembly(typeof(WallpaperCrawlerBasis))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(WallpaperCrawlerBasis)))
                .ToList();

            foreach (Type type in crawlerTypes)
            {
                var instance = (WallpaperCrawlerBasis)Activator.CreateInstance(type, null, cancellationToken, null);
                instance.Progress += Instance_Progress;
                instance.NewEntry += Instance_NewEntry;
                instance.KnownEntry += Instance_KnownEntry;
                _crawlers.Add(instance, 0);
            }
        }


        public IWallCrawlerResult Start()
        {
            //Crawling
            List<Task> tasks = new List<Task>();
            foreach (var crawler in _crawlers.Keys)
            {
                var task = Task.Factory.StartNew(crawler.Start);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());


            //Writing Report
            WallCrawlerData.Save();
            if (_reportPath != null)
            {
                try
                {
                    if (!_reportPath.Exists)
                    {
                        _reportPath.Create();
                    }
                    File.WriteAllText
                        (
                        $"{_reportPath.FullName}\\{_result.ResultName}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json",
                        _result.JSON
                        );
                }
                catch { }
            }
            OnCompleted();
            return _result;
        }
        public void StartAsync()
        {
            Task.Factory.StartNew(Start);
        }
        public async Task<IWallCrawlerResult> StartAsyncTask()
        {
            return await Task.Factory.StartNew(Start);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<int> Progress;
        private void Instance_Progress(object sender, int e)
        {
            //muss nur gerechnet werden wenn Event oder IProgress genutzt wird
            if (_progress != null || Progress != null)
            {
                lock (_progressLock)
                {
                    WallpaperCrawlerBasis instance = (WallpaperCrawlerBasis)sender;
                    _crawlers[instance] = e;

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
                                d.DynamicInvoke(this, progress);
                            }
                            else
                            {
                                syncer.BeginInvoke(d, new object[] { this, progress });  // cleanup omitted
                            }
                        }

                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<IWallEntry> KnownEntry;
        private void Instance_KnownEntry(object sender, IWallEntry e)
        {
            lock (_entryLock)
            {
                _result.AddKnownEntry(e);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<IWallEntry> NewEntry;
        private void Instance_NewEntry(object sender, IWallEntry e)
        {
            lock (_entryLock)
            {
                _result.AddNewEntry(e);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<IWallCrawlerResult> Completed;
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

    }
}
