using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPorngathererTests
    {

        PersonEntry _alettaEntry;

        [OneTimeSetUp]
        public void Setup()
        {
            _alettaEntry = new PersonCrawlerPorngatherer("Aletta Ocean", CancellationToken.None).GetPersonEntry();

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


        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(172, _alettaEntry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(58, _alettaEntry.Gewicht);
        }



        [Test]
        public void Crawler_Finds_Geburtstag()
        {
            Assert.AreEqual(new DateTime(1987, 12, 14), _alettaEntry.Geburtstag);
        }








    }
}
