using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerIStripperTests
    {
        PersonEntry _personEntry;


        [OneTimeSetUp]
        public void Setup()
        {
            var crawler = new PersonCrawlerIStripper("Aletta Ocean", CancellationToken.None);
            _personEntry = crawler.GetPersonEntry();
        }

        [Test]
        public void Crawler_Finds_Girl()
        {
            Assert.IsTrue(_personEntry.IsValid);
        }

        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual("Aletta", _personEntry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual("Ocean", _personEntry.LastName);
        }

        [Test]
        public void Crawler_Finds_Picture()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(_personEntry.PictureUrl));
        }

        [Test]
        public void Crawler_Finds_Land()
        {
            Assert.AreEqual("Hungary", _personEntry.Land);
        }

        [Test]
        public void Crawler_Finds_Geburtsort()
        {
            Assert.AreEqual("Budapest", _personEntry.Geburtsort);
        }

        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(178, _personEntry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(57, _personEntry.Gewicht);
        }

        [Test]
        public void Crawler_Finds_Maße()
        {
            Assert.AreEqual("86-66-94", _personEntry.Maße);
        }
    }
}
