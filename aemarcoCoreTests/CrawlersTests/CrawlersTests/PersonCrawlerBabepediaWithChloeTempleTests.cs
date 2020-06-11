using System;
using System.Collections.Generic;
using System.Threading;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerBabepediaWithChloeTempleTests : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerBabepedia("Chloe Temple", CancellationToken.None));
            ExpectedProfilePictures.AddRange(new List<string>
            {
                "https://www.babepedia.com/pics/Chloe%20Temple.jpg",
                "https://www.babepedia.com/pics/Chloe%20Temple2.jpg",
                "https://www.babepedia.com/pics/Chloe%20Temple3.jpg",
                "https://www.babepedia.com/pics/Chloe%20Temple4.jpg"
            });
            ExpectedBirthday = new DateTime(1998,2,6, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "United States";
            ExpectedProfession = "Porn Star";
            ExpectedAliases.AddRange(new List<string>
            {
                "Clara Fargo"
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Blonde";
            ExpectedEyeColor = "Brown";
            ExpectedMeasurements = "81-60-88";
            ExpectedCupsize = "B";
            ExpectedHeight = 160;
        }
    }
}
