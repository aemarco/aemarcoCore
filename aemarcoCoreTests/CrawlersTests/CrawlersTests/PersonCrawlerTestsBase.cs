using System.Linq;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public abstract class PersonCrawlerTestsBase
    {
        internal void SetupEntry(PersonCrawlerBase crawler)
        {
            Entry = crawler.GetPersonEntry();
            ExpectedFirstName = crawler.NameToCrawl.Split(' ').First();
            ExpectedLastName = crawler.NameToCrawl.Split(' ').Last();
        }
        internal PersonEntry Entry { get; private set; }
        internal string ExpectedFirstName { get; set; }
        internal string ExpectedLastName { get; set; }


        [Test]
        public void Entry_IsValid()
        {
            Assert.IsTrue(Entry.IsValid);
        }


        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual(ExpectedLastName, Entry.LastName);
        }
    }
}
