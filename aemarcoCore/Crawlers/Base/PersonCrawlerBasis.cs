using aemarcoCore.Common;
using aemarcoCore.Crawlers.Types;
using System;
using System.ComponentModel;
using System.Threading;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class PersonCrawlerBasis : CrawlerBasis
    {

        #region fields

        private CancellationToken _cancellationToken;
        protected string _nameToCrawl;

        #endregion

        #region ctor

        internal PersonCrawlerBasis(
            string nameToCrawl,
            CancellationToken cancellationToken)
        {
            _nameToCrawl = nameToCrawl;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region Starting 

        internal bool Start()
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

        #region Crawling

        private void DoWork()
        {
            PersonEntry entry = GetPersonEntry();
            OnProgress(100);
            if (entry.IsValid)
            {
                OnEntry(entry);
            }
        }

        internal abstract PersonEntry GetPersonEntry();

        #endregion

        #region Events

        internal event EventHandler<ProgressChangedEventArgs> Progress;
        protected void OnProgress(int prog)
        {
            Progress?.Invoke(this, new ProgressChangedEventArgs(prog, null));
        }

        internal event EventHandler<IPersonEntryEventArgs> Entry;
        private void OnEntry(IPersonEntry entry)
        {
            Entry?.Invoke(this, new IPersonEntryEventArgs { Entry = entry });
        }

        #endregion
    }
}
