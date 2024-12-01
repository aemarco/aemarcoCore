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
        ExpectedProfilePictures.AddRange([
            "https://thelordofporn.com/wp-content/uploads/2023/05/Dakota-Tyler.jpeg",
            "https://thelordofporn.com/wp-content/uploads/2023/05/Dakota-Tyler-2.jpeg"
        ]);
        ExpectedBirthday = new DateOnly(2003, 2, 20);
        ExpectedHeight = 152;
        ExpectedWeight = 48;
        ExpectedMeasurementDetails = "76A-58-83";
        //ExpectedSocialLinks.AddRange([
        //    new(SocialLinkKind.Twitter, "https://twitter.com/thedakotatyler"),
        //    new(SocialLinkKind.Instagram, "https://www.instagram.com/thedakotatyler/")
        //]);
    }
}