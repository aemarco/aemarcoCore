namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class NudevistaTestsWithAlettaOcean : PersonInfoTestsBase<Nudevista>
{
    //https://www.nudevista.at/?q=aletta+ocean&s=s
    public NudevistaTestsWithAlettaOcean()
        : base("Aletta", "Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://m95.nudevista.com/721/353721.webp");
        ExpectedAliases.AddRange([
            "Aleta Ocean", "Aletta Alien", "Aletta Florancia", "Aletta Florencia", "Aletta Madison", "Aletta Nubiles", "Aletta Nubiles -", "Aletta Sapphic", "Artemis Gold", "Beatrice P", "Dora Varga", "Doris Alien", "Jessica Kline", "Nikita Charm"
        ]);
        ExpectedGender = Gender.Female;
        ExpectedBirthday = new DateOnly(1987, 12, 14);
        ExpectedCountry = "Hungary";
        ExpectedCity = "Hungary";
        ExpectedProfession = "Porn Star, Adult Model, Webcam Model, Escort";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Black";
        ExpectedEyeColor = "Green";
        ExpectedMeasurementDetails = "86F(fake)-68-106";
        ExpectedHeight = 173;
        ExpectedWeight = 57;
        ExpectedPiercings = "Tongue, Belly Button, Clitoris, Has Piercing";
        ExpectedCareerStart = new DateOnly(2007, 1, 1);
        ExpectedStillActive = null;
        ExpectedSocialLinks.AddRange([
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/alettaoceanxxxx"),
            new SocialLink(SocialLinkKind.Instagram, "https://www.instagram.com/alettaoceanofficial1/")
        ]);

    }


}