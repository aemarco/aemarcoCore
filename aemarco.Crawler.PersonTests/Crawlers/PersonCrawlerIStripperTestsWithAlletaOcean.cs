using aemarcoCommons.PersonCrawler.Crawlers;
using aemarcoCommons.PersonCrawlerTests.Base;

namespace aemarcoCommons.PersonCrawlerTests.Crawlers
{
    internal class PersonCrawlerIStripperTestsWithAlletaOcean : PersonCrawlerTestsBase<PersonCrawlerIStripper>
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
}
