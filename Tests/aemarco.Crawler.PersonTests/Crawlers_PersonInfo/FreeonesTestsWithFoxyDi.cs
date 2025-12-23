namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;


internal class FreeonesTestsWithFoxyDi : PersonInfoTestsBase<Freeones>
{
    //https://www.freeones.at/foxy-di
    public FreeonesTestsWithFoxyDi()
        : base("Foxy", "Di")
    {

        //first and last name expected automatically

        //ExpectedProfilePictures.AddRange(new List<string>
        //{
        //    "https://media.freeones.com/freeones-photo-generated/c7/ae/VEYrLEcR4PnXXg8FGb/Teen-Nensi-B-Medina-in-Threeway-with-older-Couple_001_teaser.jpg"
        //});
        ExpectedProfilePictureMinCount = 1;
        ExpectedAliases.AddRange([

            "Angel C",
            "Foxi Di",
            "Foxy Dolce",
            "Foxy R",
            "Kleine Punci",
            "Nensi B Medina"
        ]);
        ExpectedBirthday = new DateOnly(1994, 9, 14);
        ExpectedProfession = "Adult Model,Porn Star";
        ExpectedStillActive = false;
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedCity = "St. Petersburg";
        ExpectedCountry = "Russia";

        ExpectedEthnicity = "Caucasian";
        ExpectedMeasurementDetails = "86B-60-86";
        ExpectedHeight = 154;
        ExpectedWeight = 44;
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Blue";
    }
}