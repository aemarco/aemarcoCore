using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PersonCrawlerIStripperTestsWithAlletaOcean : PersonCrawlerTestsBase<Stripper>
{

    public PersonCrawlerIStripperTestsWithAlletaOcean()
        : base("Aletta Ocean")
    {
        ExpectedCountry = "Hungary";
        ExpectedPlace = "Budapest";
        ExpectedHeight = 178;
        ExpectedWeight = 57;
        ExpectedMeasurements = "86-66-94";
        ExpectedProfilePictures.Add("http://www.istripper.com/free/sets/a0822/illustrations/full.png");
    }
}