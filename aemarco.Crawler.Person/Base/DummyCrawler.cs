using System;
using System.Threading;
using System.Threading.Tasks;
using aemarco.Crawler.Person.Common;
using aemarco.Crawler.Person.Model;

namespace aemarco.Crawler.Person.Base
{

    [PersonCrawler("DummyCrawler", int.MaxValue, isEnabled: false)]
    internal class DummyCrawler : PersonCrawlerBase
    {
        public DummyCrawler(string nameToCrawl)
            : base(nameToCrawl)
        { }

        internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
        {
            throw new Exception("Disabled DummyCrawler was started");
        }
    }
}
