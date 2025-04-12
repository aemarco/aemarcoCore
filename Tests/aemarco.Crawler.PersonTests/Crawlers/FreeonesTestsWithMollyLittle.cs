namespace aemarco.Crawler.PersonTests.Crawlers;

internal class FreeonesTestsWithMollyLittle : PersonCrawlerTestsBase<Freeones>
{

    //https://www.freeones.at/molly-little
    public FreeonesTestsWithMollyLittle()
        : base("Molly", "Little")
    {

        //first and last name expected automatically

        //ExpectedProfilePictures.AddRange(new List<string>
        //{
        //    "https://media.freeones.com/freeones-photo-generated/F5/fE/LQ6D7EQRSgmeNRzDv/mollylittle_teaser.jpg"
        //});
        ExpectedBirthday = new DateOnly(2003, 2, 10);
        ExpectedProfession = "Porn Star";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(2022, 1, 1);
        ExpectedCity = "Fairfax";
        ExpectedCountry = null;

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "81A-55-81";
        ExpectedHeight = 157;
        ExpectedWeight = 41;
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Brown";

    }
}