namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class NudevistaTestsWithAlettaOcean : PersonInfoTestsBase<Nudevista>
{
    //https://www.nudevista.at/?q=aletta+ocean&s=s
    public NudevistaTestsWithAlettaOcean()
        : base("Aletta", "Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://m99.nudevista.com/_/083/137083_370.jpg");
        ExpectedAliases.AddRange([

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
        ]);
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
        ExpectedSocialLinks.AddRange([
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/alettaoceanxxxx")
        ]);

    }


}