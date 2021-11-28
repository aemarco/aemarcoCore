using aemarco.Crawler.Wallpaper.Common;
using aemarco.Crawler.Wallpaper.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.Wallpaper.Base
{
    internal abstract class WallpaperCrawlerBasis
    {
        private readonly int _startPage;
        private readonly int _lastPage;
        private readonly bool _onlyNews;
        internal WallpaperCrawlerBasis(
            int startPage,
            int lastPage,
            bool onlyNews)
        {
            _startPage = startPage;
            _lastPage = lastPage;
            _onlyNews = onlyNews;
        }

        internal virtual bool IsAvailable => GetType().ToCrawlerInfo().IsEnabled;

        internal event EventHandler EntryFound;

        #region Starting

        // ReSharper disable once InconsistentNaming
        internal List<CrawlOffer> _crawlOffers;
        /// <summary>
        /// null or empty list means no limitation, otherwise this crawler will be 
        /// limited to offers which matching searchedCategories
        /// </summary>
        /// <param name="searchedCategories">strings of internal Category (enum)</param>
        internal void LimitAsPerFilterList(List<string> searchedCategories)
        {
            _crawlOffers ??= GetCrawlsOffers().Where(x => x.Category != null).ToList();
            _crawlOffers = _crawlOffers
                .Where(x => x.Category != null)
                .Where(o =>
                    !searchedCategories.Any() || // no filtering = allOffers
                     searchedCategories.Contains(o.Category.Category)) //filtering means offers category must be contained
                .ToList();
        }

        internal IEnumerable<CrawlOffer> GetOffers()
        {
            _crawlOffers ??= GetCrawlsOffers().Where(x => x.Category != null).ToList();
            return _crawlOffers;
        }





        /// <summary>
        /// Maps Site Category name to internal category
        /// </summary>
        /// <param name="categoryName">name of category on site</param>
        /// <returns>internal category</returns>
        protected virtual ContentCategory GetContentCategory(string categoryName)
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
                case "Ligerie beauty models":
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
                case "______Girls_Fun":
                    return new ContentCategory(Category.Girls_Fun);
            }
            return DefaultCategory;
        }

        protected virtual ContentCategory DefaultCategory => null;



        /// <summary>
        /// creates a CrawlOffer which carries the limitation infos
        /// </summary>
        /// <param name="siteCategoryName">Name of the Category on the site</param>
        /// <param name="categoryUri">Uri to navigate to the Category´s page</param>
        /// <param name="category">mapped internal category</param>
        /// <returns>CrawlOffer</returns>
        protected CrawlOffer CreateCrawlOffer(string siteCategoryName, Uri categoryUri, ContentCategory category)
        {
            return new CrawlOffer(_startPage, _lastPage, _onlyNews,
                siteCategoryName, categoryUri, category);
        }


        private List<string> _knownUrls;
        protected CancellationToken CancellationToken;
        private WallCrawlerResult _result;

        /// <summary>
        /// returns on completion, false means aborted
        /// </summary>
        /// <returns></returns>
        public async Task<WallCrawlerResult> Start(
            List<string> knownUrls,
            CancellationToken cancellationToken)
        {
            _knownUrls = knownUrls;
            CancellationToken = cancellationToken;
            _result = new WallCrawlerResult
            {
                NumberOfCrawlersInvolved = 1
            };

            await Task.Delay(100, CancellationToken)
                .ConfigureAwait(false);

            HandleAll();
            CancellationToken.ThrowIfCancellationRequested();

            return _result;
        }


        #endregion


        #region Crawling

        /// <summary>
        /// handles the entire crawler by going through offers
        /// </summary>
        private void HandleAll()
        {
            _crawlOffers ??= GetCrawlsOffers().Where(x => x.Category != null).ToList();
            foreach (var offer in _crawlOffers)
            {
                CancellationToken.ThrowIfCancellationRequested();
                HandleOffer(offer);
            }
        }
        protected abstract List<CrawlOffer> GetCrawlsOffers();

        /// <summary>
        /// handles an entire offer by going through pages
        /// </summary>
        /// <param name="catJob">CrawlOffer</param>
        protected virtual void HandleOffer(CrawlOffer catJob)
        {
            do
            {
                CancellationToken.ThrowIfCancellationRequested();
                if (!HandlePage(catJob)) catJob.ReportEndReached();
            }
            while (catJob.ContainsPagesToDo);
        }


        private int _entriesPerPage;
        /// <summary>
        /// handles current page of given offer (crawlers may override, with API for example)
        /// </summary>
        /// <param name="catJob">job to get page for</param>
        /// <returns>true if page contained any entries</returns>
        protected virtual bool HandlePage(CrawlOffer catJob)
        {
            //Seite mit Wallpaperliste
            var pageUri = GetSiteUrlForCategory(catJob);
            var doc = HtmlHelper.GetHtmlDocument(pageUri);
            var nodes = doc.DocumentNode.SelectNodes(GetSearchStringGorEntryNodes());

            //non entries on page
            if (nodes == null || !nodes.Any())
                return false;


            //report count to all classic jobs
            if (nodes.Count > _entriesPerPage)
            {
                _entriesPerPage = nodes.Count;
                _crawlOffers.ForEach(j =>
                {
                    if (j.CrawlMethod == CrawlMethod.Classic)
                        j.ReportNumberOfEntriesPerPage(_entriesPerPage);
                });
            }


            var result = false;
            //handle each node
            foreach (var node in nodes)
            {
                CancellationToken.ThrowIfCancellationRequested();

                if (catJob.ReachedMaximumStreak)
                {
                    catJob.ReportEndReached();
                    return true;
                }

                if (AddWallEntry(node, catJob))
                {
                    result = true;
                }
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
        /// adds the wall-entry to the crawlresult, as new or known wall
        /// </summary>
        /// <param name="entry">entry to add</param>
        /// <param name="catJob">job leaded to entry</param>
        protected void AddEntry(WallEntry entry, CrawlOffer catJob)
        {
            //new Entry
            if (!_knownUrls.Contains(entry.Url))
            {
                _result.NewEntries.Add(entry);
                catJob.ReportEntryDone(false);
                _knownUrls.Add(entry.Url);
            }
            //known Entry
            else
            {
                _result.KnownEntries.Add(entry);
                catJob.ReportEntryDone(true);
            }

            EntryFound?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// adds the album-entry to the crawlresult, as new or known album
        /// </summary>
        /// <param name="entry">entry to add</param>
        /// <param name="catJob">job leaded to entry</param>
        protected void AddAlbum(AlbumEntry entry, CrawlOffer catJob)
        {
            entry.HasNewEntries = entry.Entries.Any(x => !_knownUrls.Contains(x.Url));

            //bekanntes Album
            if (!entry.HasNewEntries)
            {
                _result.KnownAlbums.Add(entry);
                catJob.ReportEntryDone(true);
            }
            //neues Album
            else
            {
                _result.NewAlbums.Add(entry);
                catJob.ReportEntryDone(false);
                _knownUrls.AddRange(entry.Entries.Select(x => x.Url));
            }
            EntryFound?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }
}
