namespace aemarco.Crawler.PersonTests.Crawlers;

internal class BabepediaTestsWithFoxiDi : PersonCrawlerTestsBase<Babepedia>

{

    //https://www.babepedia.com/babe/Foxy_Di
    public BabepediaTestsWithFoxiDi()
        : base("Foxi", "Di")
    {
        ExpectedAliases.AddRange([
            "Angel C",
            "Ekaterina D",
            "Ekaterina Ivanova",
            "Inga",
            "Inna",
            "Kate",
            "Katoa",
            "Katya Ivanova",
            "Kleine Punci",
            "Marisha",
            "Medina U",
            "Nensi B"
        ]);
        //ExpectedProfilePictures.AddRange([
        //    "https://www.babepedia.com/pics/Foxy%20Di.jpg",
        //    "https://www.babepedia.com/pics/Foxy%20Di2.jpg",
        //    "https://www.babepedia.com/pics/Foxy%20Di3.jpg",
        //    "https://www.babepedia.com/pics/Foxy%20Di4.jpg"
        //]);
        ExpectedBirthday = new DateOnly(1994, 9, 14);
        ExpectedCountry = "Russia";
        ExpectedCity = "St. Petersburg";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Adult Model (Former), Porn Star (Former)";
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