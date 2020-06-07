using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPornpicsTests : PersonCrawlerTestsBase
    {

        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerPornpics("Aletta Ocean", CancellationToken.None));

            ExpectedProfilePictures.Add("https://images.pornpics.com/300/201801/18/6345351/6345351_352_d0e7.jpg");
        }
    }
}
