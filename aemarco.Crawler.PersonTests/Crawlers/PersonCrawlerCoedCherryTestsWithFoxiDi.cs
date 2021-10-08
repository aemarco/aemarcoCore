using aemarcoCommons.PersonCrawler.Crawlers;
using aemarcoCommons.PersonCrawlerTests.Base;
using System;
using System.Collections.Generic;

namespace aemarcoCommons.PersonCrawlerTests.Crawlers
{
    internal class PersonCrawlerCoedCherryTestsWithFoxiDi : PersonCrawlerTestsBase<PersonCrawlerCoedCherry>
    {

        public PersonCrawlerCoedCherryTestsWithFoxiDi()
            : base("Foxy Di")
        {
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