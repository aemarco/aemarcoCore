using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

//https://pornstarbyface.com/girls/Ariel-Rebel

internal class PersonCrawlerPornstarbyfaceWithArielRebel : PersonCrawlerTestsBase<Pornstarbyface>
{
    public PersonCrawlerPornstarbyfaceWithArielRebel()
        : base("Ariel Rebel")
    {
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://pornstarbyface.com/ImgFiles/Ariel%20Rebel/1.jpg"
        });
        ExpectedAliases.AddRange(new List<string>
        {
            "Ariel Rebel Unplugged",
            "Miss Rebel",
            "Ariel Nubiles",
            "Ariel"
        });
        ExpectedCountry = "Canada";
        ExpectedCity = "Quebec";
        ExpectedBirthday = new DateTime(1988, 3, 14, 0, 0, 0, DateTimeKind.Utc);
        //ignore sign
        ExpectedEthnicity = "Caucasian";
        ExpectedEyeColor = "Hazelnuts";
        ExpectedHairColor = "Brunette";
        ExpectedHeight = 152;
        ExpectedWeight = 44;
        ExpectedCupsize = "A";
        ExpectedMeasurements = "86-58-81";
    }
}
