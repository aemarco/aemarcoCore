namespace aemarco.Crawler.PersonTests.Crawlers;

internal class BabepediaTestsWithChloeTemple : PersonCrawlerTestsBase<Babepedia>
{

    //https://www.babepedia.com/babe/Chloe_Temple
    public BabepediaTestsWithChloeTemple()
        : base("Chloe", "Temple")
    {

        //first and last name expected automatically
        //ExpectedProfilePictures.AddRange([
        //    "https://www.babepedia.com/pics/Chloe%20Temple.jpg",
        //    "https://www.babepedia.com/pics/Chloe%20Temple2.jpg",
        //    "https://www.babepedia.com/pics/Chloe%20Temple3.jpg",
        //    "https://www.babepedia.com/pics/Chloe%20Temple4.jpg"
        //]);


        ExpectedSocialLinks =
        [
            new SocialLink(SocialLinkKind.Official, "https://link.me/senorita_satan"),
            new SocialLink(SocialLinkKind.Twitter, "https://x.com/demonspawnn"),
            new SocialLink(SocialLinkKind.Instagram, "https://instagram.com/Senorita_Satan"),
            new SocialLink(SocialLinkKind.OnlyFans, "https://onlyfans.com/chloe_temple")
        ];
        ExpectedAliases.AddRange([
            "Clara Fargo",
            "Larissa",
            "Senorita Satan",
            "Sharon Purcell"
        ]);
        ExpectedRating = true;
        ExpectedBirthday = new DateOnly(1998, 2, 6);
        ExpectedCountry = "United States";
        ExpectedCity = "Cincinnati";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Adult Model, Escort, Internet Personality, Porn Star";
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