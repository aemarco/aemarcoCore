using aemarco.Crawler.Core.Attributes;
using aemarco.Crawler.Core.Extensions;
using aemarcoCommons.PersonCrawler.Model;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.PersonCrawlerTests.Base
{
    internal abstract class PersonCrawlerTestsBase<T>
    {
        private readonly string _nameToCrawl;

        protected PersonCrawlerTestsBase(string nameToCrawl)
        {
            ExpectedFirstName = nameToCrawl.Split(' ')[0];
            ExpectedLastName = nameToCrawl.Split(' ')[1];
            _nameToCrawl = nameToCrawl;
        }

        [OneTimeSetUp]
        public void Init()
        {
            var type = PersonCrawler.PersonCrawler
                    .GetCrawlerTypes()
                    .FirstOrDefault(x => x.FullName == typeof(T).FullName);
            Info = type.ToCrawlerInfo();

            if (!Info.IsEnabled)
                return;


            var crawler = new PersonCrawler.PersonCrawler(_nameToCrawl);
            crawler.AddPersonSiteFilter(Info.FriendlyName);
            Entry = crawler.StartAsync().GetAwaiter().GetResult();
        }

        internal PersonCrawlerAttribute Info { get; private set; }
        internal PersonInfo Entry { get; private set; }



        [Test]
        public void Entry_Matches_PersonSite()
        {
            if (Entry is null) return;
            Assert.AreEqual(Info.FriendlyName, Entry.PersonEntrySource);
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
            Assert.AreEqual(ExpectedBirthday!.Value, Entry.Birthday);
        }

        internal DateTime? ExpectedCareerStart { get; set; }
        [Test]
        public void Crawler_Finds_CareerStart()
        {
            if (Entry is null) return;
            if (!ExpectedCareerStart.HasValue) return;
            Assert.AreEqual(ExpectedCareerStart!.Value, Entry.CareerStart);
        }



        internal string ExpectedCountry { get; set; }
        [Test]
        public void Crawler_Finds_Country()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedCountry)) return;
            Assert.AreEqual(ExpectedCountry, Entry.Country);
        }

        internal string ExpectedPlace { get; set; }
        [Test]
        public void Crawler_Finds_Place()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedPlace)) return;
            Assert.AreEqual(ExpectedPlace, Entry.City);
        }

        internal string ExpectedProfession { get; set; }
        [Test]
        public void Crawler_Finds_Profession()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedProfession)) return;
            Assert.AreEqual(ExpectedProfession, Entry.Profession);
        }
        internal string ExpectedEthnicity { get; set; }
        [Test]
        public void Crawler_Finds_Ethnicity()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedEthnicity)) return;
            Assert.AreEqual(ExpectedEthnicity, Entry.Ethnicity);
        }

        internal string ExpectedHairColor { get; set; }
        [Test]
        public void Crawler_Finds_Hair()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedHairColor)) return;
            Assert.AreEqual(ExpectedHairColor, Entry.HairColor);
        }


        internal string ExpectedEyeColor { get; set; }
        [Test]
        public void Crawler_Finds_Eyes()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedEyeColor)) return;
            Assert.AreEqual(ExpectedEyeColor, Entry.EyeColor);
        }

        internal int? ExpectedHeight { get; set; }
        [Test]
        public void Crawler_Finds_Height()
        {
            if (Entry is null) return;
            if (!ExpectedHeight.HasValue) return;
            Assert.AreEqual(ExpectedHeight!.Value, Entry.Height);
        }

        internal int? ExpectedWeight { get; set; }
        [Test]
        public void Crawler_Finds_Weight()
        {
            if (Entry is null) return;
            if (!ExpectedWeight.HasValue) return;
            Assert.AreEqual(ExpectedWeight!.Value, Entry.Weight);
        }

        internal string ExpectedMeasurements { get; set; }
        [Test]
        public void Crawler_Finds_Measurements()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedMeasurements)) return;
            Assert.AreEqual(ExpectedMeasurements, Entry.Measurements);
        }

        internal string ExpectedCupsize { get; set; }
        [Test]
        public void Crawler_Finds_CupSize()
        {
            if (Entry is null) return;
            if (string.IsNullOrWhiteSpace(ExpectedCupsize)) return;
            Assert.AreEqual(ExpectedCupsize, Entry.CupSize);
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
                Assert.IsTrue(Entry.Aliases.Contains(al));
            }
        }


    }
}
