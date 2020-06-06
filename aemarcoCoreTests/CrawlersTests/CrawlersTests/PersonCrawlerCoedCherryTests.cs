using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerCoedCherryTests
    {
        PersonEntry _foxiEntry;
        PersonEntry _chloeEntry;

        [OneTimeSetUp]
        public void Setup()
        {
            _foxiEntry = new PersonCrawlerCoedCherry("Foxi Di", CancellationToken.None).GetPersonEntry();
            _chloeEntry = new PersonCrawlerCoedCherry("Chloe Brooke", CancellationToken.None).GetPersonEntry();
        }

        [Test]
        public void Crawler_Finds_Girl()
        {
            Assert.IsTrue(_foxiEntry.IsValid);
        }

        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual("Foxy", _foxiEntry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual("Di", _foxiEntry.LastName);
        }


        [Test]
        public void Crawler_Finds_Geburtstag()
        {
            Assert.AreEqual(new DateTime(1994, 09, 14), _foxiEntry.Geburtstag);
        }


        [Test]
        public void Crawler_Finds_Land()
        {
            Assert.AreEqual("Russia", _foxiEntry.Land);
        }

        [Test]
        public void Crawler_Finds_Geburtsort()
        {
            Assert.AreEqual("Savannah", _chloeEntry.Geburtsort);
        }


        [Test]
        public void Crawler_Finds_Beruf()
        {
            Assert.AreEqual("Porn Star", _foxiEntry.Beruf);
        }

        [Test]
        public void Crawler_Finds_Karrierestatus()
        {
            Assert.AreEqual("active", _foxiEntry.Karrierestatus);
        }



        [Test]
        public void Crawler_Finds_Aliase()
        {
            List<string> als = new List<string>
            {
                "Nensi B",
                "Medina U",
                "Nensi B Medina",
                "Kate X-Art",
                "Foxi Di",
                "Katoa Erotic Beauty",
                "Nensi Amour Angels",
                "Nensi Show Beauty",
                "Katoa Errotica Archives"
            };
            foreach (var al in als)
            {
                Assert.IsTrue(_foxiEntry.Aliase.Contains(al));
            }
        }






        [Test]
        public void Crawler_Finds_Rasse()
        {
            Assert.AreEqual("Caucasian", _foxiEntry.Rasse);
        }



        [Test]
        public void Crawler_Finds_Haare()
        {
            Assert.AreEqual("Brown", _foxiEntry.Haare);
        }

        [Test]
        public void Crawler_Finds_Augen()
        {
            Assert.AreEqual("Blue", _foxiEntry.Augen);
        }


        [Test]
        public void Crawler_Finds_Maße()
        {
            Assert.AreEqual("86-61-86", _foxiEntry.Maße);
        }
        [Test]
        public void Crawler_Finds_Körbchengröße()
        {
            Assert.AreEqual("B", _foxiEntry.Körbchengröße);
        }

        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(165, _foxiEntry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(44, _foxiEntry.Gewicht);
        }



        [Test]
        public void Crawler_Finds_Piercings()
        {
            Assert.AreEqual("Belly button", _foxiEntry.Piercings);
        }

    }
}
