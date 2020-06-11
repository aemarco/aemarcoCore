using System;
using System.Collections.Generic;
using System.Threading;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerBabepediaWithFoxiDiTests : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerBabepedia("Foxi Di", CancellationToken.None));
            ExpectedFirstName = "Foxy";
            ExpectedLastName = "Di";
            ExpectedProfilePictures.AddRange(new List<string>
            {
                "https://www.babepedia.com/pics/Foxy%20Di.jpg",
                "https://www.babepedia.com/pics/Foxy%20Di2.jpg",
                "https://www.babepedia.com/pics/Foxy%20Di3.jpg",
                "https://www.babepedia.com/pics/Foxy%20Di4.jpg",
                "https://www.babepedia.com/pics/Foxy%20Di6.jpg",
                "https://www.babepedia.com/pics/Foxy%20Di7.jpg"
            });
            ExpectedBirthday = new DateTime(1994,9,14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "Russian Federation";
            ExpectedPlace = "St. Petersburg";
            ExpectedProfession = "Adult Model, Porn Star";
            ExpectedCareerStart = new DateTime(2013,1,1,0,0,0, DateTimeKind.Utc);
            ExpectedStillActive = true;
            ExpectedAliases.AddRange(new List<string>
            {
                "Foxi Di",
                "Kleine Punci",
                "Nensi B Medina",
                "Katya Ivanova",
                "Katoa",
                "Angel C",
                "Ekaterina D"
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Brown";
            ExpectedEyeColor = "Hazel";
            ExpectedMeasurements = "86-60-86";
            ExpectedCupsize = "B";
            ExpectedHeight = 165;
            ExpectedWeight = 45;
        }
    }
}
