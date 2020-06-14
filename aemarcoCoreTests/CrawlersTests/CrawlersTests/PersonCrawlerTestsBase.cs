using System;
using System.Collections.Generic;
using System.Linq;
using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using FluentAssertions;
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
            _expectedPersonSite = crawler.PersonSite.ToString();

            _byPassTests = crawler.PersonSite.IsDisabled();
        }

        private bool _byPassTests;
        private string _expectedPersonSite;

        [Test]
        public void Entry_Matches_PersonSite()
        {
            if (_byPassTests) Assert.Pass();
            Assert.AreEqual(_expectedPersonSite, Entry.PersonEntrySource);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        internal PersonEntry Entry { get; private set; }
        [Test]
        public void Entry_IsValid()
        {
            if (_byPassTests) Assert.Pass();
            Assert.IsTrue(Entry.IsValid);
        }


        internal string ExpectedFirstName { get; set; }
        [Test]
        public void Crawler_Finds_FirstName()
        {
            if (_byPassTests) Assert.Pass();
            Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
        }

        internal string ExpectedLastName { get; set; }
        [Test]
        public void Crawler_Finds_LastName()
        {
            if (_byPassTests) Assert.Pass();
            Assert.AreEqual(ExpectedLastName, Entry.LastName);
        }

        internal DateTime? ExpectedBirthday { get; set; }
        [Test]
        public void Crawler_Finds_Birthday()
        {
            if (_byPassTests) Assert.Pass();
            if (!ExpectedBirthday.HasValue) Assert.Pass();
            Assert.AreEqual(ExpectedBirthday!.Value, Entry.Geburtstag);
        }

        internal DateTime? ExpectedCareerStart { get; set; }
        [Test]
        public void Crawler_Finds_CareerStart()
        {
            if (_byPassTests) Assert.Pass();
            if (!ExpectedCareerStart.HasValue) Assert.Pass();
            Assert.AreEqual(ExpectedCareerStart!.Value, Entry.Karrierestart);
        }



        internal string ExpectedCountry { get; set; }
        [Test]
        public void Crawler_Finds_Country()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedCountry)) Assert.Pass();
            Assert.AreEqual(ExpectedCountry, Entry.Land);
        }

        internal string ExpectedPlace { get; set; }
        [Test]
        public void Crawler_Finds_Place()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedPlace)) Assert.Pass();
            Assert.AreEqual(ExpectedPlace, Entry.Geburtsort);
        }
        
        internal string ExpectedProfession { get; set; }
        [Test]
        public void Crawler_Finds_Profession()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedProfession)) Assert.Pass();
            Assert.AreEqual(ExpectedProfession, Entry.Beruf);
        }
        internal string ExpectedEthnicity { get; set; }
        [Test]
        public void Crawler_Finds_Ethnicity()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedEthnicity)) Assert.Pass();
            Assert.AreEqual(ExpectedEthnicity, Entry.Rasse);
        }
        
        internal string ExpectedHairColor { get; set; }
        [Test]
        public void Crawler_Finds_Hair()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedHairColor)) Assert.Pass();
            Assert.AreEqual(ExpectedHairColor, Entry.Haare);
        }

        
        internal string ExpectedEyeColor { get; set; }
        [Test]
        public void Crawler_Finds_Eyes()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedEyeColor)) Assert.Pass();
            Assert.AreEqual(ExpectedEyeColor, Entry.Augen);
        }

        internal int? ExpectedHeight { get; set; }
        [Test]
        public void Crawler_Finds_Height()
        {
            if (_byPassTests) Assert.Pass();
            if (!ExpectedHeight.HasValue) Assert.Pass();
            Assert.AreEqual(ExpectedHeight!.Value, Entry.Größe);
        }

        internal int? ExpectedWeight { get; set; }
        [Test]
        public void Crawler_Finds_Weight()
        {
            if (_byPassTests) Assert.Pass();
            if (!ExpectedWeight.HasValue) Assert.Pass();
            Assert.AreEqual(ExpectedWeight!.Value, Entry.Gewicht);
        }

        internal string ExpectedMeasurements { get; set; }
        [Test]
        public void Crawler_Finds_Measurements()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedMeasurements)) Assert.Pass();
            Assert.AreEqual(ExpectedMeasurements, Entry.Maße);
        }

        internal string ExpectedCupsize { get; set; }
        [Test]
        public void Crawler_Finds_CupSize()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedCupsize)) Assert.Pass();
            Assert.AreEqual(ExpectedCupsize, Entry.Körbchengröße);
        }

        internal string ExpectedPiercings { get; set; }
        [Test]
        public void Crawler_Finds_Piercings()
        {
            if (_byPassTests) Assert.Pass();
            if (string.IsNullOrWhiteSpace(ExpectedPiercings)) Assert.Pass();
            Assert.AreEqual(ExpectedPiercings, Entry.Piercings);
        }

        internal bool? ExpectedStillActive { get; set; }
        [Test]
        public void Crawler_Finds_StillActive()
        {
            if (_byPassTests) Assert.Pass();
            Assert.AreEqual(ExpectedStillActive, Entry.StillActive);
        }

        internal List<string> ExpectedProfilePictures { get; set; } = new List<string>();
        [Test]
        public void Crawler_Finds_Pictures()
        {
            if (_byPassTests) Assert.Pass();
            foreach (var url in ExpectedProfilePictures)
            {
                Entry.ProfilePictures.Should().Contain(x => x.Url == url);
            }
        }

        internal List<string> ExpectedAliases { get; set; } = new List<string>();
        [Test]
        public void Crawler_Finds_Aliases()
        {
            if (_byPassTests) Assert.Pass();
            foreach (var al in ExpectedAliases)
            {
                Assert.IsTrue(Entry.Aliase.Contains(al));
            }
        }


    }
}
