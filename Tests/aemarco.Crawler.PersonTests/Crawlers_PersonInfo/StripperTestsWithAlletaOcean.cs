namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class StripperTestsWithAlletaOcean : PersonInfoTestsBase<Stripper>
{
    //https://www.istripper.com/de/models/Aletta-Ocean
    public StripperTestsWithAlletaOcean()
        : base("Aletta", "Ocean")
    {
        //first and last name expected automatically
        ExpectedRating = true;
        ExpectedGender = Gender.Female; //gender is assumed
        ExpectedProfilePictures.Add(
            "http://www.istripper.com/free/sets/a0822/illustrations/full.png");
        ExpectedCountry = "Hungary";
        ExpectedCity = "Budapest";
        ExpectedHeight = 178;
        ExpectedWeight = 57;
        ExpectedMeasurementDetails = "86-66-94";
    }
}