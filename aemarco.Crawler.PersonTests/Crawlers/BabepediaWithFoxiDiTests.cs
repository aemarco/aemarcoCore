using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class BabepediaWithFoxiDiTests : PersonCrawlerTestsBase<Babepedia>
{

    //https://www.babepedia.com/babe/Foxy_Di
    public BabepediaWithFoxiDiTests()
        : base("Foxi Di")
    {
        //first and last name expected automatically
        ExpectedFirstName = "Foxy";
        ExpectedAliases.AddRange(new List<string>
        {
            "Foxi Di",
            "Kleine Punci",
            "Nensi B",
            "Katya Ivanova",
            "Katoa",
            "Angel C",
            "Ekaterina D",
            "Medina U"
        });
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://www.babepedia.com/pics/Foxy%20Di.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di2.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di3.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di4.jpg"
        });
        ExpectedBirthday = new DateOnly(1994, 9, 14);
        ExpectedCountry = "Russian Federation";
        ExpectedCity = "St. Petersburg";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Glamour Model (Former), Porn Star (Former)";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Hazel";
        ExpectedHeight = 157;
        ExpectedWeight = 45;
        ExpectedMeasurementDetails = "86B-60-86";
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedStillActive = false;
        ExpectedPiercings = "Navel";
    }
}