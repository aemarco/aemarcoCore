namespace aemarco.Crawler.PersonTests.Crawlers;

internal class TheLordOfPornTestsWithDakotaTyler : PersonCrawlerTestsBase<TheLordOfPorn>
{

    //https://thelordofporn.com/pornstars/dakota-tyler
    public TheLordOfPornTestsWithDakotaTyler()
        : base("Dakota", "Tyler")
    {
        //first and last name expected automatically

        ExpectedRating = true;
        ExpectedGender = Gender.Female; //gender is assumed
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://thelordofporn.com/wp-content/uploads/2023/05/Dakota-Tyler.jpeg",
            "https://thelordofporn.com/wp-content/uploads/2023/05/Dakota-Tyler-2.jpeg"
        });
        ExpectedBirthday = new DateOnly(2003, 2, 20);
        ExpectedHeight = 152;
        ExpectedWeight = 48;
        ExpectedMeasurementDetails = "76A-58-83";
        ExpectedSocialLinks.AddRange(new List<SocialLink>
        {
            new(SocialLinkKind.Twitter, "https://twitter.com/thedakotatyler"),
            new(SocialLinkKind.Instagram, "https://www.instagram.com/thedakotatyler/")
        });
    }
}

internal class TheLordOfPornTestsWithAngelaWhite : PersonCrawlerTestsBase<TheLordOfPorn>
{

    //https://thelordofporn.com/pornstars/angela-white
    public TheLordOfPornTestsWithAngelaWhite()
        : base("Angela", "White")
    {
        //first and last name expected automatically

        ExpectedRating = true;
        ExpectedGender = Gender.Female; //gender is assumed
        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://thelordofporn.com/wp-content/uploads/2017/03/Angela-White-2-208x300.jpg",
            "https://thelordofporn.com/wp-content/uploads/2017/03/Angela-White-208x300.jpg"
        });
        ExpectedAliases.AddRange(new[]
        {
            "Angie"
        });
        ExpectedBirthday = new DateOnly(1985, 3, 4);
        ExpectedHeight = 160;
        ExpectedWeight = 55;
        ExpectedMeasurementDetails = "106G-68-104";
        ExpectedSocialLinks.AddRange(new List<SocialLink>
        {
            new(SocialLinkKind.Official, "http://angelawhite.com/"),
            new(SocialLinkKind.Twitter, "https://twitter.com/angelawhite"),
            new(SocialLinkKind.Instagram, "https://www.instagram.com/theangelawhite")
        });
    }
}
