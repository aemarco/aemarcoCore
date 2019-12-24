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
        #region ctor

        private readonly int _startPage;
        private readonly int _lastPage;
        private readonly bool _onlyNews;
        protected CancellationToken _cancellationToken;


        internal WallpaperCrawlerBasis(
            int startPage,
            int lastPage,
            bool onlyNews,
            CancellationToken cancellationToken)
        {
            _startPage = startPage;
            _lastPage = lastPage;
            _onlyNews = onlyNews;
            _cancellationToken = cancellationToken;
        }


        #endregion

        #region props

        internal abstract SourceSite SourceSite { get; }


        private List<CrawlOffer> _catJobs;
        internal bool HasWorkingOffers
        {
            get
            {
                _catJobs = _catJobs ?? GetCrawlsOffers();
                return _catJobs.Any();
            }
        }

        #endregion

        #region Starting 

        /// <summary>
        /// null or empty list means no limitation, otherwise this crawler will be 
        /// limited to offers which matching searchedCategories
        /// </summary>
        /// <param name="searchedCategories">strings of internal Category (enum)</param>
        internal void LimitAsPerFilterlist(List<string> searchedCategories)
        {
            _catJobs = GetCrawlsOffers()
                 .Where(o =>
                    searchedCategories == null || // no filtering = allOffers
                    !searchedCategories.Any() || // no filtering = allOffers
                    (o.Category?.Category != null && searchedCategories.Contains(o.Category.Category))) //filtering means offer must be contained
                 .ToList();
        }




        /// <summary>
        /// Maps Site Category name to internal category
        /// </summary>
        /// <param name="categoryName">name of category on site</param>
        /// <returns>internal category</returns>
        protected virtual IContentCategory GetContentCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "Erotic Wallpapers":
                case "Erotic Girls":
                    return new ContentCategory(Category.Girls);
                case "Celebrities":
                    return new ContentCategory(Category.Girls_Celebrities);
                case "Celebrity Fakes":
                    return new ContentCategory(Category.Girls_CelebrityFakes);
                case "Girls & Cars":
                    return new ContentCategory(Category.Girls_Cars);
                case "Girls & Bikes":
                    return new ContentCategory(Category.Girls_Bikes);
                case "______Girls_Guns":
                    return new ContentCategory(Category.Girls_Guns);
                case "Anime":
                case "Fantasy Girls":
                case "3D & Vector Girls":
                    return new ContentCategory(Category.Girls_Fantasy);
                case "Cosplay":
                    return new ContentCategory(Category.Girls_Cosplay);
                case "Lingerie":
                case "Lingerie Models":
                case "Lingerie Girls":
                case "lingerie":
                case "underwear":
                case "stockings":
                    return new ContentCategory(Category.Girls_Lingerie);
                case "Lesbian":
                case "Lesbians":
                    return new ContentCategory(Category.Girls_Lesbians);
                case "Beach":
                case "Girls & Beaches":
                    return new ContentCategory(Category.Girls_Beaches);
                case "Asian Girls":
                case "asian":
                case "Asian":
                case "Indian":
                case "Japanese":
                case "Thai":
                    return new ContentCategory(Category.Girls_Asian);
                case "Holidays":
                case "Christmas":
                    return new ContentCategory(Category.Girls_Holidays);
                case "Fetish Girls":
                case "Bondage":
                case "Blindfold":
                case "Latex":
                    return new ContentCategory(Category.Girls_Fetish);
                case "______Girls_Blowjob":
                    return new ContentCategory(Category.Girls_Blowjob);
                case "Hardcore":
                    return new ContentCategory(Category.Girls_Hardcore);
                case "Homemade":
                case "Amateur":
                    return new ContentCategory(Category.Girls_Amateur);
                case "Gloryhole":
                    return new ContentCategory(Category.Girls_Gloryhole);
                case "Selfpics":
                case "Self Shot":
                    return new ContentCategory(Category.Girls_Selfies);
            }
            return DefaultCategory;
        }

        protected virtual IContentCategory DefaultCategory => null;








        /// <summary>
        /// creates a CrawlOffer which carries the limitation infos
        /// </summary>
        /// <param name="siteCategoryName">Name of the Category on the site</param>
        /// <param name="categoryUri">Uri to navigate to the Category´s page</param>
        /// <param name="category">mapped internal category</param>
        /// <returns>CrawlOffer</returns>
        protected CrawlOffer CreateCrawlOffer(
            string siteCategoryName, Uri categoryUri, IContentCategory category)
        {
            return new CrawlOffer(_startPage, _lastPage, _onlyNews,
                siteCategoryName, categoryUri, category);
        }



        /// <summary>
        /// returns on completion, false means aborted
        /// </summary>
        /// <returns></returns>
        public bool Start(WallpaperCrawler crawler)
        {
            try
            {
                //Crawling
                DoWork();
                _cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                crawler.AddException(ex);
            }
            return true;
        }


        #endregion

        #region Events

        internal event EventHandler<ProgressChangedEventArgs> Progress;
        protected void OnProgress()
        {
            if (Progress != null)
            {
                int done = _catJobs.Sum(x => x.DoneEntries);
                int todo = _catJobs.Sum(x => x.TotalEntries);
                int prog = (todo == 0) ? 0 : Convert.ToInt32(100.0 * done / todo);

                Progress(this, new ProgressChangedEventArgs(prog, null));
            }
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

        #region Crawling

        /// <summary>
        /// handles the entire crawler by going through offers
        /// </summary>
        private void DoWork()
        {
            if (_catJobs == null)
                _catJobs = GetCrawlsOffers();

            foreach (var catJob in _catJobs)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                GetCategory(catJob);
            }
        }
        protected abstract List<CrawlOffer> GetCrawlsOffers();

        /// <summary>
        /// handles an entire offer by going through pages
        /// </summary>
        /// <param name="catJob">CrawlOffer</param>
        protected virtual void GetCategory(CrawlOffer catJob)
        {
            do
            {
                _cancellationToken.ThrowIfCancellationRequested();
                if (!GetPage(catJob)) catJob.ReportEndReached();
            }
            while (catJob.ContainsPagesToDo);
            //no more work
            OnProgress();
        }


        private int _entriesPerPage;
        /// <summary>
        /// handles current page of given offer (crawlers may override, with API for example)
        /// </summary>
        /// <param name="catJob">job to get page for</param>
        /// <returns>true if page contained any entries</returns>
        protected virtual bool GetPage(CrawlOffer catJob)
        {
            bool result = false;

            //Seite mit Wallpaperliste
            Uri pageUri = GetSiteUrlForCategory(catJob);
            var doc = GetDocument(pageUri);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(GetSearchStringGorEntryNodes());

            //non entries on page
            if (nodes == null || !nodes.Any())
                return result;


            //report count to all classic jobs
            if (nodes.Count > _entriesPerPage)
            {
                _entriesPerPage = nodes.Count;
                _catJobs.ForEach(j =>
                {
                    if (j.CrawlMethod == CrawlMethod.Classic)
                        j.ReportNumberOfEntriesPerPage(_entriesPerPage);
                });
            }

            //handle each node
            foreach (HtmlNode node in nodes)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                if (catJob.ReachedMaximumStreak)
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

        /// <summary>
        /// Gets the url for the site containing the category
        /// </summary>
        /// <param name="catJob">current job</param>
        /// <returns>Uri to navigate for category</returns>
        protected abstract Uri GetSiteUrlForCategory(CrawlOffer catJob);
        /// <summary>
        /// Gets search path for entry nodes
        /// </summary>
        /// <returns>search path</returns>
        protected abstract string GetSearchStringGorEntryNodes();

        /// <summary>
        /// handles 1 entry in a page
        /// </summary>
        /// <param name="node">Html node for entry</param>
        /// <param name="catJob">current job</param>
        /// <returns>true if found a valid entry</returns>
        protected abstract bool AddWallEntry(HtmlNode node, CrawlOffer catJob);


        /// <summary>
        /// adds the entry to the crawlresult, as new or known
        /// </summary>
        /// <param name="entry">entry to add</param>
        /// <param name="catJob">job leaded to entry</param>
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
