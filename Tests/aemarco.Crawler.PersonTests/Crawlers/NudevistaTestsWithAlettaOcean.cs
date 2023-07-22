using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.Person.Model;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class NudevistaTestsWithAlettaOcean : PersonCrawlerTestsBase<Nudevista>
{
    //https://www.nudevista.at/?q=aletta+ocean&s=s
    public NudevistaTestsWithAlettaOcean()
        : base("Aletta Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/083/137083_370.jpg");
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
        ExpectedGender = Gender.Female;
        ExpectedBirthday = new DateOnly(1987, 12, 14);
        ExpectedCountry = "Hungary";
        ExpectedCity = "Budapest";
        ExpectedProfession = "Pornstar";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Black";
        ExpectedEyeColor = "Green";
        ExpectedMeasurementDetails = "96D-66-93";
        ExpectedHeight = 172;
        ExpectedWeight = 59;
        ExpectedPiercings = "Clit, Navel, Tongue";
        ExpectedCareerStart = new DateOnly(2007, 1, 1);
        ExpectedStillActive = null;
    }


}