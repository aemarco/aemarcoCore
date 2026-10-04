namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class FreeonesTestsWithArielRebel : PersonInfoTestsBase<Freeones>
{

    //https://www.freeones.at/ariel-rebel
    public FreeonesTestsWithArielRebel()
        : base("Ariel", "Rebel")
    {

        //first and last name expected automatically

        //ExpectedProfilePictures.AddRange(new List<string>
        //{
        //    "http://www.sexy-models.net/galleries/12/pics18259/0_big.jpg"
        //});
        //ExpectedAliases.AddRange([

        //    "The Rebel"
        //]);
        //ExpectedSocialLinks.AddRange(new List<SocialLink>
        //{
        //    new(SocialLinkKind.Twitter, "https://twitter.com/thedakotatyler")
        //});
        ExpectedBirthday = new DateOnly(1985, 9, 23);
        ExpectedProfession = "Porn Star,Adult Model,Cam Girl";
        ExpectedStillActive = false;
        ExpectedCareerStart = new DateOnly(2005, 1, 1);
        ExpectedCity = "Montreal";
        ExpectedCountry = null;

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "91A-58-81";
        ExpectedHeight = 154;
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedPiercings = "Eyebrow ring (currently removed to prevent infection)";


    }
}