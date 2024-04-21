namespace aemarco.Crawler.PersonTests.Crawlers;

internal class NudevistaTestsWithAmberSym : PersonCrawlerTestsBase<Nudevista>
{

    //https://www.nudevista.at/?q=Amber+Sym&s=s
    public NudevistaTestsWithAmberSym()
        : base("Amber", "Sym")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/319/145319_370.jpg");
        ExpectedAliases.AddRange([
            "Amber Symm",
            "Ohc Destiny",
            "Tara Marie",
            "Tara Marie Price",
            "Tara P."
        ]);
        ExpectedGender = Gender.Female;
        ExpectedBirthday = new DateOnly(1989, 11, 4);
        ExpectedCountry = "USA";
        ExpectedProfession = "Fashion Model";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedMeasurementDetails = "86C(fake)-60-86";
        ExpectedHeight = 167;
        ExpectedWeight = 48;
        ExpectedPiercings = "Ears, Navel";
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedStillActive = false;
        ExpectedSocialLinks.AddRange([
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/amber2sym"),
            new SocialLink(SocialLinkKind.YouTube, "https://www.youtube.com/user/AmberSym")
        ]);

    }
}