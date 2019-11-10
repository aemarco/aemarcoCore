using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerNudevistaTests
    {

        PersonEntry _alettaEntry;
        PersonEntry _amberEntry;

        [OneTimeSetUp]
        public void Setup()
        {
            _alettaEntry = new PersonCrawlerNudevista("Aletta Ocean", CancellationToken.None).GetPersonEntry();
            _amberEntry = new PersonCrawlerNudevista("Amber Sym", CancellationToken.None).GetPersonEntry();
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
        public void Crawler_Finds_Land()
        {
            Assert.AreEqual("Hungary", _alettaEntry.Land);
        }

        [Test]
        public void Crawler_Finds_Geburtsort()
        {
            Assert.AreEqual("Budapest", _alettaEntry.Geburtsort);
        }

        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(172, _alettaEntry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(59, _alettaEntry.Gewicht);
        }

        [Test]
        public void Crawler_Finds_Maße()
        {
            Assert.AreEqual("96-66-93", _alettaEntry.Maße);
        }


        [Test]
        public void Crawler_Finds_Geburtstag()
        {
            Assert.AreEqual(new DateTime(1987, 12, 14), _alettaEntry.Geburtstag);
        }

        [Test]
        public void Crawler_Finds_Beruf()
        {
            Assert.AreEqual("Pornstar", _alettaEntry.Beruf);
        }

        [Test]
        public void Crawler_Finds_Karrierestart()
        {
            Assert.AreEqual(new DateTime(2007, 1, 1), _alettaEntry.Karrierestart);
        }



        [Test]
        public void Crawler_Finds_Aliase()
        {
            List<string> als = new List<string>
            {
                "Aletta Alien",
                "Aletta Atk",
                "Aletta Florancia",
                "Aletta Florencia",
                "Aletta Nubiles",
                "Aletta Sapphic",
                "Artemis Gold",
                "Beatrice P",
                "Dora Varga",
                "Doris Alien",
                "Jessica Kline",
                "Nikita Charm",
            };
            foreach (var al in als)
            {
                Assert.IsTrue(_alettaEntry.Aliase.Contains(al));
            }
        }

        [Test]
        public void Crawler_Finds_Karrierestatus()
        {
            Assert.AreEqual("Retired", _amberEntry.Karrierestatus);
        }


        [Test]
        public void Crawler_Finds_Rasse()
        {
            Assert.AreEqual("Caucasian", _alettaEntry.Rasse);
        }

        [Test]
        public void Crawler_Finds_Haare()
        {
            Assert.AreEqual("Black", _alettaEntry.Haare);
        }

        [Test]
        public void Crawler_Finds_Augen()
        {
            Assert.AreEqual("Green", _alettaEntry.Augen);
        }

        [Test]
        public void Crawler_Finds_Körbchengröße()
        {
            Assert.AreEqual("D (fake)", _alettaEntry.Körbchengröße);
        }


        [Test]
        public void Crawler_Finds_Piercings()
        {
            Assert.AreEqual("Clit, Navel, Tongue", _alettaEntry.Piercings);
        }
    }
}
