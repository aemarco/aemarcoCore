namespace aemarco.Crawler.PersonTests.Crawlers;

internal class MilffoxTestsWithJuliaAnn : PersonCrawlerTestsBase<Milffox>
{

    //https://www.milffox.com/milf-pornstars/Carolina-Sweets/
    public MilffoxTestsWithJuliaAnn()
        : base("Julia", "Ann")
    {
        //first and last name expected automatically

        ExpectedProfilePictures =
        [
            "https://s1.milffox.com/t/ps/1/6/130_normal.jpg"
        ];

        ExpectedBirthday = new DateOnly(1969, 10, 8);
        ExpectedCity = "Los Angeles";
        ExpectedCountry = "United States";
        ExpectedEthnicity = "Caucasian";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(1992, 1, 1);
        ExpectedAliases =
        [
            "Julie Ann",
            "Julia Ann Tavella",
            "Julia Tavella"
        ];
        ExpectedSocialLinks =
        [
            new SocialLink(
                SocialLinkKind.Facebook,
                "https://www.facebook.com/TheRealJuliaAnn"),
            new SocialLink(
                SocialLinkKind.Twitter,
                "https://twitter.com/therealJuliaAnn"),
            new SocialLink(
                SocialLinkKind.Instagram,
                "https://instagram.com/therealjuliaannlive")
        ];


        ExpectedMeasurementDetails = "36E(fake)-24-38";
        ExpectedWeight = 58;
        ExpectedHeight = 172;
        ExpectedEyeColor = "Blue";
        ExpectedHairColor = "Blonde";
        ExpectedPiercings = "Tattoo";
    }
}