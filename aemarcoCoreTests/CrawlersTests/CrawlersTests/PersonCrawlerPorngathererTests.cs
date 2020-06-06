using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Threading;
using FluentAssertions;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPorngathererTests
    {

        PersonEntry _entry;

        [OneTimeSetUp]
        public void Setup()
        {
            _entry = new PersonCrawlerPorngatherer("Aletta Ocean", CancellationToken.None).GetPersonEntry();

        }

        [Test]
        public void Crawler_Finds_Girl()
        {
            Assert.IsTrue(_entry.IsValid);
        }

        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual("Aletta", _entry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual("Ocean", _entry.LastName);
        }

        [Test]
        public void Crawler_Finds_Picture()
        {
            _entry.ProfilePictures.Should().HaveCount(1);
            
        }


        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(172, _entry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(58, _entry.Gewicht);
        }



        [Test]
        public void Crawler_Finds_Geburtstag()
        {
            Assert.AreEqual(new DateTime(1987, 12, 14), _entry.Geburtstag);
        }








    }
}
