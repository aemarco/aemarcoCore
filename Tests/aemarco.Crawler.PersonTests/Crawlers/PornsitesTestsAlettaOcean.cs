namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PornsitesTestsAlettaOcean : PersonCrawlerTestsBase<Pornsites>
{

    //https://pornsites.xxx/pornstars/Aletta-Ocean
    public PornsitesTestsAlettaOcean()
        : base("Aletta Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add(
            "https://cdn.pornsites.xxx/models/6394/aletta-ocean-4.jpg");
        ExpectedBirthday = new DateOnly(1987, 12, 14);
        ExpectedGender = Gender.Female;
        ExpectedHairColor = "Black";
        ExpectedEyeColor = "Grey";
        ExpectedMeasurementDetails = "96DDD-66-93";
        ExpectedWeight = 58;
        ExpectedHeight = 172;
        ExpectedEthnicity = "Caucasian";
        ExpectedCountry = "Hungary";
    }
}