using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerIStripperTestsWithAlletaOcean : PersonCrawlerTestsBase
    {
        
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry( new PersonCrawlerIStripper("Aletta Ocean", CancellationToken.None));

            ExpectedCountry = "Hungary";
            ExpectedPlace = "Budapest";
            ExpectedHeight = 178;
            ExpectedWeight = 57;
            ExpectedMeasurements = "86-66-94";
            ExpectedProfilePictures.Add("http://www.istripper.com/free/sets/a0822/illustrations/full.png");
        }
    }
}
