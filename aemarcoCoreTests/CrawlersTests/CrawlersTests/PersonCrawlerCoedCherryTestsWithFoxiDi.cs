using System;
using System.Collections.Generic;
using System.Threading;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerCoedCherryTestsWithFoxiDi : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerCoedCherry("Foxy Di", CancellationToken.None));

            ExpectedBirthday = new DateTime(1994, 9, 14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "Russia";
            ExpectedProfession = "Porn Star";
            ExpectedStillActive = true;
            ExpectedAliases.AddRange(new List<string>
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
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Brown";
            ExpectedEyeColor = "Blue";
            ExpectedHeight = 165;
            ExpectedWeight = 44;
            ExpectedMeasurements = "86-61-86";
            ExpectedCupsize = "B";
            ExpectedPiercings = "Belly button";
        }
    }
}