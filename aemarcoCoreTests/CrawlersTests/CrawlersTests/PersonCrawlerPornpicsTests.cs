using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPornpicsTests
    {

        PersonEntry _alettaEntry;

        [OneTimeSetUp]
        public void Setup()
        {
            _alettaEntry = new PersonCrawlerPornpics("Aletta Ocean", CancellationToken.None).GetPersonEntry();

        }

        [Test]
        public void Crawler_Finds_Girl()
        {
            Assert.IsTrue(_alettaEntry.IsValid);
        }

        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual("Aletta", _alettaEntry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual("Ocean", _alettaEntry.LastName);
        }

        [Test]
        public void Crawler_Finds_Picture()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(_alettaEntry.PictureUrl));
        }


    }
}
