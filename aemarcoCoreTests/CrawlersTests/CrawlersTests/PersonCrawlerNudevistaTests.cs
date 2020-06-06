using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerNudevistaTests
    {

        PersonEntry _entry;
        PersonEntry _amberEntry;

        [OneTimeSetUp]
        public void Setup()
        {
            _entry = new PersonCrawlerNudevista("Aletta Ocean", CancellationToken.None).GetPersonEntry();
            _amberEntry = new PersonCrawlerNudevista("Amber Sym", CancellationToken.None).GetPersonEntry();
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
        public void Crawler_Finds_Land()
        {
            Assert.AreEqual("Hungary", _entry.Land);
        }

        [Test]
        public void Crawler_Finds_Geburtsort()
        {
            Assert.AreEqual("Budapest", _entry.Geburtsort);
        }

        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(172, _entry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(59, _entry.Gewicht);
        }

        [Test]
        public void Crawler_Finds_Maße()
        {
            Assert.AreEqual("96-66-93", _entry.Maße);
        }


        [Test]
        public void Crawler_Finds_Geburtstag()
        {
            Assert.AreEqual(new DateTime(1987, 12, 14), _entry.Geburtstag);
        }

        [Test]
        public void Crawler_Finds_Beruf()
        {
            Assert.AreEqual("Pornstar", _entry.Beruf);
        }

        [Test]
        public void Crawler_Finds_Karrierestart()
        {
            Assert.AreEqual(new DateTime(2007, 1, 1), _entry.Karrierestart);
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
                Assert.IsTrue(_entry.Aliase.Contains(al));
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
            Assert.AreEqual("Caucasian", _entry.Rasse);
        }

        [Test]
        public void Crawler_Finds_Haare()
        {
            Assert.AreEqual("Black", _entry.Haare);
        }

        [Test]
        public void Crawler_Finds_Augen()
        {
            Assert.AreEqual("Green", _entry.Augen);
        }

        [Test]
        public void Crawler_Finds_Körbchengröße()
        {
            Assert.AreEqual("D (fake)", _entry.Körbchengröße);
        }


        [Test]
        public void Crawler_Finds_Piercings()
        {
            Assert.AreEqual("Clit, Navel, Tongue", _entry.Piercings);
        }
    }
}
