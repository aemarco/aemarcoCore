using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers
{
    internal class PersonCrawlerCoedCherryTestsWithChloeBrooke : PersonCrawlerTestsBase<PersonCrawlerCoedCherry>
    {
        public PersonCrawlerCoedCherryTestsWithChloeBrooke()
            : base("Chloe Brooke")
        {
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
