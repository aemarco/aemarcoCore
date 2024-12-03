namespace aemarco.Crawler.PersonTests.Crawlers;
internal class MilffoxTestsWithCarolinaSweets : PersonCrawlerTestsBase<Milffox>
{

    //https://www.milffox.com/milf-pornstars/Carolina-Sweets/
    public MilffoxTestsWithCarolinaSweets()
        : base("Carolina", "Sweets")
    {
        //first and last name expected automatically

        ExpectedProfilePictures =
        [
            "https://s1.milffox.com/t/ps/1/23/5532_normal.jpg"
        ];

        ExpectedBirthday = new DateOnly(1996, 9, 29);
        ExpectedCity = "Chicago";
        ExpectedCountry = "United States";
        ExpectedEthnicity = "Caucasian";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(2016, 1, 1);
        ExpectedAliases =
        [
            "Ariana Flores",
            "Madelyn Leonard"
        ];
        ExpectedSocialLinks =
        [
            new SocialLink(
                SocialLinkKind.Twitter,
                "https://twitter.com/xcarolinasweets")
        ];


        ExpectedMeasurementDetails = "86B-61-96";
        ExpectedWeight = 53;
        ExpectedHeight = 154;
        ExpectedEyeColor = "Blue";
        ExpectedHairColor = "Blonde";
        ExpectedPiercings = "Navel";
    }
}