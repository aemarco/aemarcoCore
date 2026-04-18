namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class NudevistaTestsWithAmberSym : PersonInfoTestsBase<Nudevista>
{

    //https://www.nudevista.at/?q=Amber+Sym&s=s
    public NudevistaTestsWithAmberSym()
        : base("Amber", "Sym")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://m99.nudevista.com/800/353800.webp");
        ExpectedAliases.AddRange([
            "Amber Symm", "Destiny 2", "Ohc Destiny", "Tara Dane", "Tara Marie", "Tara Marie 1", "Tara Marie Price", "Tara P", "Tara Price", "Tarra Marie", "Tarra Realitykings Com"
        ]);
        ExpectedGender = Gender.Female;
        ExpectedBirthday = new DateOnly(1989, 11, 4);
        ExpectedCountry = "United States";
        ExpectedProfession = "Adult Model, Radio Host";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedMeasurementDetails = "86C-60-86";
        ExpectedHeight = 168;
        ExpectedWeight = 48;
        ExpectedPiercings = "Navel, Ears";
        ExpectedCareerStart = new DateOnly(2012, 1, 1);
        ExpectedStillActive = true;
        ExpectedSocialLinks.AddRange([
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/amber2sym"),
            new SocialLink(SocialLinkKind.YouTube, "https://www.youtube.com/user/AmberSym"),
            new SocialLink(SocialLinkKind.Facebook, "https://www.facebook.com/SimplySym")
        ]);

    }
}