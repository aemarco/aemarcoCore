using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public abstract class PersonCrawlerTestsBase
    {


        internal void SetupEntry(PersonCrawlerBase crawler)
        {
            if (crawler.PersonSite.IsDisabled()) return;


            Entry = crawler.GetPersonEntry();
            ExpectedFirstName = crawler.NameToCrawl.Split(' ').First();
            ExpectedLastName = crawler.NameToCrawl.Split(' ').Last();
            _expectedPersonSite = crawler.PersonSite.ToString();
        }


        private string _expectedPersonSite;

        [Test]
        public void Entry_Matches_PersonSite()
        {
            if (Entry is null) return;
            Assert.AreEqual(_expectedPersonSite, Entry.PersonEntrySource);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        internal PersonEntry Entry { get; private set; }
        [Test]
        public void Entry_IsValid()
        {
            if (Entry is null) return;
            Assert.IsTrue(Entry.IsValid);
        }


        internal string ExpectedFirstName { get; set; }
        [Test]
        public void Crawler_Finds_FirstName()
        {
            if (Entry is null) return;
            Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
        }

        internal string ExpectedLastName { get; set; }
        [Test]
        public void Crawler_Finds_LastName()
        {
            if (Entry is null) return;
            Assert.AreEqual(ExpectedLastName, Entry.LastName);
        }

        internal DateTime? ExpectedBirthday { get; set; }
        [Test]
        public void Crawler_Finds_Birthday()
        {
            if (Entry is null) return;
            if (!ExpectedBirthday.HasValue) return;
            Assert.AreEqual(ExpectedBirthday!.Value, Entry.Geburtstag);
        }

        internal DateTime? ExpectedCareerStart { get; set; }
        [Test]
        public void Crawler_Finds_CareerStart()
        {
            if (Entry is null) return;
            if (!ExpectedCareerStart.HasValue) return;
            Assert.AreEqual(ExpectedCareerStart!.Value, Entry.Karrierestart);
        }



        internal string ExpectedCountry { get; set; }
        [Test]
        public void Crawler_Finds_Country()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedCountry)) return;
            Assert.AreEqual(ExpectedCountry, Entry.Land);
        }

        internal string ExpectedPlace { get; set; }
        [Test]
        public void Crawler_Finds_Place()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedPlace)) return;
            Assert.AreEqual(ExpectedPlace, Entry.Geburtsort);
        }

        internal string ExpectedProfession { get; set; }
        [Test]
        public void Crawler_Finds_Profession()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedProfession)) return;
            Assert.AreEqual(ExpectedProfession, Entry.Beruf);
        }
        internal string ExpectedEthnicity { get; set; }
        [Test]
        public void Crawler_Finds_Ethnicity()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedEthnicity)) return;
            Assert.AreEqual(ExpectedEthnicity, Entry.Rasse);
        }

        internal string ExpectedHairColor { get; set; }
        [Test]
        public void Crawler_Finds_Hair()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedHairColor)) return;
            Assert.AreEqual(ExpectedHairColor, Entry.Haare);
        }


        internal string ExpectedEyeColor { get; set; }
        [Test]
        public void Crawler_Finds_Eyes()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedEyeColor)) return;
            Assert.AreEqual(ExpectedEyeColor, Entry.Augen);
        }

        internal int? ExpectedHeight { get; set; }
        [Test]
        public void Crawler_Finds_Height()
        {
            if (Entry is null) return;
            if (!ExpectedHeight.HasValue) return;
            Assert.AreEqual(ExpectedHeight!.Value, Entry.Größe);
        }

        internal int? ExpectedWeight { get; set; }
        [Test]
        public void Crawler_Finds_Weight()
        {
            if (Entry is null) return;
            if (!ExpectedWeight.HasValue) return;
            Assert.AreEqual(ExpectedWeight!.Value, Entry.Gewicht);
        }

        internal string ExpectedMeasurements { get; set; }
        [Test]
        public void Crawler_Finds_Measurements()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedMeasurements)) return;
            Assert.AreEqual(ExpectedMeasurements, Entry.Maße);
        }

        internal string ExpectedCupsize { get; set; }
        [Test]
        public void Crawler_Finds_CupSize()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedCupsize)) return;
            Assert.AreEqual(ExpectedCupsize, Entry.Körbchengröße);
        }

        internal string ExpectedPiercings { get; set; }
        [Test]
        public void Crawler_Finds_Piercings()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedPiercings)) return;
            Assert.AreEqual(ExpectedPiercings, Entry.Piercings);
        }

        internal bool? ExpectedStillActive { get; set; }
        [Test]
        public void Crawler_Finds_StillActive()
        {
            if (Entry is null) return;
            Assert.AreEqual(ExpectedStillActive, Entry.StillActive);
        }

        internal List<string> ExpectedProfilePictures { get; set; } = new List<string>();
        [Test]
        public void Crawler_Finds_Pictures()
        {
            if (Entry is null) return;
            foreach (var url in ExpectedProfilePictures)
            {
                Entry.ProfilePictures.Should().Contain(x => x.Url == url);
            }
        }

        internal List<string> ExpectedAliases { get; set; } = new List<string>();
        [Test]
        public void Crawler_Finds_Aliases()
        {
            if (Entry is null) return;
            foreach (var al in ExpectedAliases)
            {
                Assert.IsTrue(Entry.Aliase.Contains(al));
            }
        }


    }
}
