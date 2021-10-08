using aemarco.Crawler.Core.Extensions;
using aemarcoCommons.PersonCrawler.Base;
using aemarcoCommons.PersonCrawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.PersonCrawler
{
    public class PersonCrawler
    {
        private readonly string _nameToCrawl;
        private readonly List<string> _filterPersonSites = new List<string>();

        /// <summary>
        /// Used to crawl various Websites for a Person
        /// </summary>
        /// <param name="nameToCrawl">firstname lastname for the person</param>
        public PersonCrawler(string nameToCrawl)
        {
            _nameToCrawl = nameToCrawl;
        }


        public IEnumerable<string> GetAvailableCrawlers()
        {
            var crawlerInfos = GetCrawlerTypes()
               .Select(x => x.ToCrawlerInfo())
               .Where(x => x.IsEnabled)
               .OrderBy(x => x.Priority)
               .ToList();

            foreach (var crawlerInfo in crawlerInfos)
            {
                yield return crawlerInfo.FriendlyName;
            }
        }

        /// <summary>
        /// Not using means all sites will be crawled
        /// Using means only sites added will be crawled.
        /// Use GetAvailableCrawlers, to know which ones are supported
        /// </summary>
        /// <param name="crawler">crawler to include</param>
        public void AddPersonSiteFilter(string crawler)
        {
            if (!_filterPersonSites.Contains(crawler))
            {
                _filterPersonSites.Add(crawler);
            }
        }

        /// <summary>
        /// Do the crawling :)
        /// </summary>
        /// <returns>composed PersonEntry</returns>
        public async Task<PersonInfo> StartAsync(CancellationToken cancellationToken = default)
        {
            //start all crawlers
            var tasks = new List<Task<PersonInfo>>();
            foreach (var type in GetCrawlerTypes())
            {
                var info = type.ToCrawlerInfo();
                //skip filtered
                if (_filterPersonSites.Count > 0 && !_filterPersonSites.Contains(info.FriendlyName))
                    continue;
                //skip disabled
                if (!info.IsEnabled)
                    continue;

                var crawler = (PersonCrawlerBase)Activator.CreateInstance(type, _nameToCrawl);
                tasks.Add(Task.Run(() => crawler.GetPersonEntry(cancellationToken), cancellationToken));
            }
            await Task.WhenAll(tasks);

            var entries = new List<PersonInfo>();
            foreach (var task in tasks)
            {
                var personInfo = await task;
                entries.Add(personInfo);
            }

            PersonInfo result = null;
            foreach (var entry in entries
                .OrderBy(x => x.PersonEntryPriority))
            {
                //first entry
                if (result == null)
                {
                    result = entry;
                    continue;
                }

                result.Merge(entry);
            }
            return result;
        }

        internal static List<Type> GetCrawlerTypes()
        {
            var types = System.Reflection.Assembly
                .GetAssembly(typeof(PersonCrawlerBase))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(PersonCrawlerBase)))
                .ToList();
            return types;
        }

    }



}
