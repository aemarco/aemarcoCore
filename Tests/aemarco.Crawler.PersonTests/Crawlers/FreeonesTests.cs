namespace aemarco.Crawler.PersonTests.Crawlers;

internal class FreeonesTestsWithFoxyDi : PersonCrawlerTestsBase<Freeones>
{
    //https://www.freeones.at/foxy-di
    public FreeonesTestsWithFoxyDi()
        : base("Foxy", "Di")
    {

        //first and last name expected automatically

        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://media.freeones.com/freeones-photo-generated/qW/CS/t4o3rHCGjRCkVTapum/Hottie-Nensi-B-Medina-bending-over-in-the-Shower_005_teaser.jpg"
        });
        ExpectedAliases.AddRange(new[]
        {
            "Angel C",
            "Foxi Di",
            "Foxy Dolce",
            "Foxy R",
            "Kleine Punci",
            "Nensi B Medina"
        });
        ExpectedBirthday = new DateOnly(1994, 9, 14);
        ExpectedProfession = "Adult Models,Porn Stars";
        ExpectedStillActive = false;
        ExpectedCareerStart = new DateOnly(2012, 1, 1);
        ExpectedCity = "St. Petersburg";
        ExpectedCountry = "Russian";

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "86B-60-86";
        ExpectedHeight = 154;
        ExpectedWeight = 44;
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Blue";
    }
}

internal class FreeonesTestsWithArielRebel : PersonCrawlerTestsBase<Freeones>
{

    //https://www.freeones.at/ariel-rebel
    public FreeonesTestsWithArielRebel()
        : base("Ariel", "Rebel")
    {

        //first and last name expected automatically

        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://media.freeones.com/freeones-photo-generated/mF/Ld/W3hd7LgmvGsZtLwwaK/Ariel-Rebel-smile-brunette-001_teaser.jpg"
        });
        ExpectedAliases.AddRange(new[]
        {
            "The Rebel"
        });
        //ExpectedSocialLinks.AddRange(new List<SocialLink>
        //{
        //    new(SocialLinkKind.Twitter, "https://twitter.com/thedakotatyler")
        //});
        ExpectedBirthday = new DateOnly(1985, 9, 23);
        ExpectedProfession = "Porn Stars";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(2005, 1, 1);
        ExpectedCity = "Montreal";
        ExpectedCountry = null;

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "A";
        ExpectedHeight = 154;
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedPiercings = "Eyebrow ring (currently removed to prevent infection)";


    }
}

internal class FreeonesTestsWithMollyLittle : PersonCrawlerTestsBase<Freeones>
{

    //https://www.freeones.at/molly-little
    public FreeonesTestsWithMollyLittle()
        : base("Molly", "Little")
    {

        //first and last name expected automatically

        ExpectedProfilePictures.AddRange(new List<string>
        {
            "https://media.freeones.com/freeones-photo-generated/F5/fE/LQ6D7EQRSgmeNRzDv/mollylittle_teaser.jpg"
        });
        ExpectedBirthday = new DateOnly(2003, 2, 10);
        ExpectedProfession = "Porn Stars";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(2022, 1, 1);
        ExpectedCity = "Fairfax";
        ExpectedCountry = null;

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "81A-55-81";
        ExpectedHeight = 157;
        ExpectedWeight = 40;
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Brown";

    }
}
