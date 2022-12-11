using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;


//https://www.babepedia.com/babe/Foxy_Di


internal class PersonCrawlerBabepediaWithFoxiDiTests : PersonCrawlerTestsBase<Babepedia>
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
        ExpectedCity = "St. Petersburg";
        ExpectedProfession = "Glamour Model (former), Porn Star (former)";
        ExpectedCareerStart = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        ExpectedStillActive = false;
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
        ExpectedHeight = 157;
        ExpectedWeight = 45;
    }
}