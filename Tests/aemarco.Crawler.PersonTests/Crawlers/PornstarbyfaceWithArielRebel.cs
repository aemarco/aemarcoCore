namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PornstarbyfaceWithArielRebel : PersonCrawlerTestsBase<Pornstarbyface>
{

    //https://pornstarbyface.com/girls/Ariel-Rebel
    public PornstarbyfaceWithArielRebel()
        : base("Ariel Rebel")
    {
        //first and last name expected automatically
        ExpectedGender = Gender.Female; //gender is assumed
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
        ExpectedBirthday = new DateOnly(1988, 3, 14);
        //ignore sign
        ExpectedEthnicity = "Caucasian";
        ExpectedEyeColor = "Hazelnuts";
        ExpectedHairColor = "Brunette";
        ExpectedHeight = 157;
        ExpectedWeight = 44;
        ExpectedMeasurementDetails = "86A-58-81";
    }
}
