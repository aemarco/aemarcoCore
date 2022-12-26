using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PersonCrawlerIStripperTestsWithAlletaOcean : PersonCrawlerTestsBase<Stripper>
{
    //https://www.istripper.com/de/models/Aletta-Ocean
    public PersonCrawlerIStripperTestsWithAlletaOcean()
        : base("Aletta Ocean")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add(
            "http://www.istripper.com/free/sets/a0822/illustrations/full.png");
        ExpectedCountry = "Hungary";
        ExpectedCity = "Budapest";
        ExpectedHeight = 178;
        ExpectedWeight = 57;
        ExpectedMeasurementDetails = "86-66-94";
    }
}