using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class BabepediaWithChloeTempleTests : PersonCrawlerTestsBase<Babepedia>
{

    //https://www.babepedia.com/babe/Chloe_Temple
    public BabepediaWithChloeTempleTests()
        : base("Chloe Temple")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://www.babepedia.com/pics/Chloe%20Temple.jpg",
            "https://www.babepedia.com/pics/Chloe%20Temple2.jpg",
            "https://www.babepedia.com/pics/Chloe%20Temple3.jpg",
        });
        ExpectedAliases.AddRange(new List<string>
        {
            "Clara Fargo",
            "Larissa"
        });
        ExpectedBirthday = new DateOnly(2000, 2, 6);
        ExpectedCountry = "United States";
        ExpectedCity = "Arizona";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Adult Model, Porn Star";
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Brown";
        ExpectedHeight = 160;
        ExpectedWeight = 46;
        ExpectedMeasurementDetails = "81B-60-88";
        ExpectedCareerStart = new DateOnly(2018, 1, 1);
        ExpectedStillActive = true;
        ExpectedPiercings = "Both Nipples";
    }
}