using aemarco.Crawler.Core.Attributes;
using aemarcoCommons.PersonCrawler.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.PersonCrawler.Base
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
