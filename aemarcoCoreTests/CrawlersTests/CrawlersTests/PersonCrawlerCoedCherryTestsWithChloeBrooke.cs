using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerCoedCherryTestsWithChloeBrooke : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerCoedCherry("Chloe Brooke", CancellationToken.None));

            ExpectedBirthday = new DateTime(1994, 5, 10, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "United States";
            ExpectedPlace = "Savannah";
            ExpectedProfession = "Porn Star";
            ExpectedStillActive = true;
            ExpectedAliases.AddRange(new List<string>
            {
                "Chloee Brooke"
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Blonde";
            ExpectedEyeColor = "Green";
            ExpectedHeight = 167;
            ExpectedWeight = 49;
            ExpectedMeasurements = "32-29-34";
            ExpectedCupsize = "A";
        }
    }
}
