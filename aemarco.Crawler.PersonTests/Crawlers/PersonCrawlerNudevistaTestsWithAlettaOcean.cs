using aemarcoCommons.PersonCrawler.Crawlers;
using aemarcoCommons.PersonCrawlerTests.Base;
using System;
using System.Collections.Generic;

namespace aemarcoCommons.PersonCrawlerTests.Crawlers
{
    internal class PersonCrawlerNudevistaTestsWithAlettaOcean : PersonCrawlerTestsBase<PersonCrawlerNudevista>
    {

        public PersonCrawlerNudevistaTestsWithAlettaOcean()
            : base("Aletta Ocean")
        {
            ExpectedProfilePictures.Add("https://b99.nudevista.com/_/083/137083_370.jpg");
            ExpectedBirthday = new DateTime(1987, 12, 14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "Hungary";
            ExpectedPlace = "Budapest";
            ExpectedProfession = "Pornstar";
            ExpectedCareerStart = new DateTime(2007, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            ExpectedStillActive = null;
            ExpectedAliases.AddRange(new List<string>
            {
                "Aletta Alien",
                "Aletta Atk",
                "Aletta Florancia",
                "Aletta Florencia",
                "Aletta Nubiles",
                "Aletta Sapphic",
                "Artemis Gold",
                "Beatrice P",
                "Dora Varga",
                "Doris Alien",
                "Jessica Kline",
                "Nikita Charm"
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Black";
            ExpectedEyeColor = "Green";
            ExpectedMeasurements = "96-66-93";
            ExpectedCupsize = "D (fake)";
            ExpectedHeight = 172;
            ExpectedWeight = 59;
            ExpectedPiercings = "Clit, Navel, Tongue";
        }


    }
}
