using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;
internal class BabesAndStarsWithArielRebelTests : PersonCrawlerTestsBase<BabesAndStars>
{
    //https://www.babesandstars.com/a/ariel-rebel/
    public BabesAndStarsWithArielRebelTests()
        : base("Ariel Rebel")
    {
        //first and last name expected automatically
        ExpectedAliases.AddRange(new List<string>
        {
            "Ariel",
            "Ariel Nubile",
            "Ariel Nubiles",
            "Ariel Rabel",
            "Ariel Rebel Unplugged",
            "Arielrebel",
            "Miss Rebel",
            "Rebel Mayhem",
            "Rebels",
            "Rebelsownthenet",
            "The Rebel"
        });
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://static.babesandstars.com/models/2000/2094/250x330.jpg"
        });

        ExpectedCountry = "Canada";
        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "86A-58-81";
        ExpectedBirthday = new DateOnly(1988, 3, 14);
        ExpectedEyeColor = "Brown";
        ExpectedWeight = 48;
        ExpectedHeight = 157;
        ExpectedHairColor = "Brunette";
    }
}
