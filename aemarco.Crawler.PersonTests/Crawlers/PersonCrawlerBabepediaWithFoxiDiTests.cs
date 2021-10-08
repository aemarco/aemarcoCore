using aemarcoCommons.PersonCrawler.Crawlers;
using aemarcoCommons.PersonCrawlerTests.Base;
using System;
using System.Collections.Generic;

namespace aemarcoCommons.PersonCrawlerTests.Crawlers
{
    internal class PersonCrawlerBabepediaWithFoxiDiTests : PersonCrawlerTestsBase<PersonCrawlerBabepedia>
    {
        public PersonCrawlerBabepediaWithFoxiDiTests()
            : base("Foxi Di")
        {
            ExpectedFirstName = "Foxy";
            ExpectedProfilePictures.AddRange(new List<string>
                {
                    "https://www.babepedia.com/pics/Foxy%20Di.jpg",
                    "https://www.babepedia.com/pics/Foxy%20Di2.jpg",
                    "https://www.babepedia.com/pics/Foxy%20Di3.jpg",
                    "https://www.babepedia.com/pics/Foxy%20Di4.jpg"
                });
            ExpectedBirthday = new DateTime(1994, 9, 14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "Russian Federation";
            ExpectedPlace = "St. Petersburg";
            ExpectedProfession = "Porn Star";
            ExpectedCareerStart = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ExpectedStillActive = true;
            ExpectedAliases.AddRange(new List<string>
                {
                    "Foxi Di",
                    "Kleine Punci",
                    "Nensi B",
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
