namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class FreeonesTestsWithMollyLittle : PersonInfoTestsBase<Freeones>
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
        ExpectedProfession = "Influencer,Adult Model,Porn Star";
        ExpectedStillActive = true;
        ExpectedCareerStart = new DateOnly(2022, 1, 1);
        ExpectedCity = "Fairfax";
        ExpectedCountry = "United States";

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "81A-55-81";
        ExpectedHeight = 160;
        ExpectedWeight = 49;
        ExpectedHairColor = "Blonde";
        ExpectedEyeColor = "Brown";

    }
}