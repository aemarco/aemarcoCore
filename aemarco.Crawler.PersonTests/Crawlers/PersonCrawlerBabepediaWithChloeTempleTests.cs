using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers
{
    internal class PersonCrawlerBabepediaWithChloeTempleTests : PersonCrawlerTestsBase<PersonCrawlerBabepedia>
    {
        public PersonCrawlerBabepediaWithChloeTempleTests()
            : base("Chloe Temple")
        {
            ExpectedProfilePictures.AddRange(new List<string>
            {
                "https://www.babepedia.com/pics/Chloe%20Temple.jpg",
                "https://www.babepedia.com/pics/Chloe%20Temple2.jpg",
                "https://www.babepedia.com/pics/Chloe%20Temple3.jpg",
            });
            ExpectedBirthday = new DateTime(2000, 2, 6, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "United States";
            ExpectedProfession = "Adult Model, Porn Star";
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
            ExpectedStillActive = true;
        }
    }
}
