#pragma warning disable CS0612
namespace aemarco.Crawler.PersonTests.Crawlers;




internal class BabepediaTestsWithFoxiDi : PersonCrawlerTestsBase<Babepedia>

{

    //https://www.babepedia.com/babe/Foxy_Di
    public BabepediaTestsWithFoxiDi()
        : base("Foxi", "Di")
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


internal class BabepediaTestsWithChloeTemple : PersonCrawlerTestsBase<Babepedia>
{

    //https://www.babepedia.com/babe/Chloe_Temple
    public BabepediaTestsWithChloeTemple()
        : base("Chloe", "Temple")
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
        ExpectedBirthday = new DateOnly(1998, 2, 6);
        ExpectedCountry = "United States";
        ExpectedCity = "Arizona";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Adult Model, Porn Star";
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Brown";
        ExpectedHeight = 160;
        ExpectedWeight = 45;
        ExpectedMeasurementDetails = "81B-60-88";
        ExpectedCareerStart = new DateOnly(2018, 1, 1);
        ExpectedStillActive = true;
        ExpectedPiercings = "Both Nipples";
    }
}