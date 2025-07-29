namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class XxxBiosWithHannahHays : PersonInfoTestsBase<XxxBios>
{

    //https://xxxbios.com/hannah-hays-biography
    public XxxBiosWithHannahHays()
        : base("Hannah", "Hays")
    {
        //first and last name expected automatically

        ExpectedCity = "Georgia (USA)";
        ExpectedCountry = "United States";
        ExpectedMeasurementDetails = "81B-60-73";
        ExpectedHeight = 157;
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Green";
        ExpectedPiercings = "Navel and tongue";
        ExpectedCareerStart = new DateOnly(2017, 1, 1);
        ExpectedStillActive = true;
        ExpectedProfilePictures.AddRange([
            "https://xxxbios.com/wp-content/uploads/2018/05/wildoncam_hannahhays_02-e1526416848792.jpg",
            "https://xxxbios.com/wp-content/uploads/2018/05/53f2b4621e1ca421e5cb97bd079ae1ca-e1526416870561.jpg",
            "https://xxxbios.com/wp-content/uploads/2018/05/wildoncam_hannahhays_11-e1526416954787.jpg",
            "https://xxxbios.com/wp-content/uploads/2018/05/420-3.jpg",
            "https://xxxbios.com/wp-content/uploads/2018/05/772201701596111475_3000-e1526417099590.jpg"
        ]);
        ExpectedSocialLinks.AddRange([

            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/hannahhaysxxx"),
            new SocialLink(SocialLinkKind.Instagram, "https://www.instagram.com/hannahhaysxx/")
        ]);
    }

}
