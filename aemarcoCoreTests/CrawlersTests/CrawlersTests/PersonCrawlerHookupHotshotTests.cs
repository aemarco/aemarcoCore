using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerHookupHotshotTests : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerHookupHotshot("Kenzie Reeves", CancellationToken.None));

            ExpectedProfilePictures.Add("https://hookuphotshot.com/wp-content/uploads/2020/03/BLK3527.jpg");
            ExpectedProfilePictures.Add("https://hookuphotshot.com/wp-content/uploads/2020/03/BLK4145.jpg");
        }

       

    }
}
