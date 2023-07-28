namespace aemarco.Crawler.PersonTests.Crawlers;

#pragma warning disable CS0612

internal class BabepediaWithFoxiDiTests : PersonCrawlerTestsBase<Babepedia>
{

    //https://www.babepedia.com/babe/Foxy_Di
    public BabepediaWithFoxiDiTests()
        : base("Foxi Di")
    {
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
        ExpectedProfession = "Adult Model, Glamour Model, Porn Star";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Hazel";
        ExpectedHeight = 157;
        ExpectedWeight = 49;
        ExpectedMeasurementDetails = "86B-60-86";
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedStillActive = true;
        ExpectedPiercings = "Navel";
    }
}