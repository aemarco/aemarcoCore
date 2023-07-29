namespace aemarco.Crawler.PersonTests.Crawlers;

internal class NudevistaTestsWithAlettaOcean : PersonCrawlerTestsBase<Nudevista>
{
    //https://www.nudevista.at/?q=aletta+ocean&s=s
    public NudevistaTestsWithAlettaOcean()
        : base("Aletta Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/083/137083_370.jpg");
        ExpectedAliases.AddRange(new List<string>
        {
            "Aletta Alien",
            "Aletta Atk",
            "Aletta Florancia",
            "Aletta Florencia",
            "Aletta Nubiles",
            "Aletta Sapphic",
            "Artemis Gold",
            "Beatrice P",
            "Dora Varga",
            "Doris Alien",
            "Jessica Kline",
            "Nikita Charm"
        });
        ExpectedGender = Gender.Female;
        ExpectedBirthday = new DateOnly(1987, 12, 14);
        ExpectedCountry = "Hungary";
        ExpectedCity = "Budapest";
        ExpectedProfession = "Pornstar";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Black";
        ExpectedEyeColor = "Green";
        ExpectedMeasurementDetails = "96D(fake)-66-93";
        ExpectedHeight = 172;
        ExpectedWeight = 59;
        ExpectedPiercings = "Clit, Navel, Tongue";
        ExpectedCareerStart = new DateOnly(2007, 1, 1);
        ExpectedStillActive = null;
        ExpectedSocialLinks.AddRange(new[]
        {
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/alettaoceanxxxx")
        });

    }


}

internal class NudevistaTestsWithAmberSym : PersonCrawlerTestsBase<Nudevista>
{

    //https://www.nudevista.at/?q=Amber+Sym&s=s
    public NudevistaTestsWithAmberSym()
        : base("Amber Sym")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/319/145319_370.jpg");
        ExpectedAliases.AddRange(new List<string>
        {
            "Amber Symm",
            "Ohc Destiny",
            "Tara Marie",
            "Tara Marie Price",
            "Tara P."
        });
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
        ExpectedSocialLinks.AddRange(new[]
        {
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/amber2sym"),
            new SocialLink(SocialLinkKind.YouTube, "https://www.youtube.com/user/AmberSym")
        });

    }
}