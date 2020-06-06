using System.Linq;
using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System.Threading;
using FluentAssertions;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerHookupHotshotTests : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerHookupHotshot("Kenzie Reeves", CancellationToken.None));
        }

        [Test]
        public void Crawler_Finds_CutePicture()
        {
            Entry.ProfilePictures.Should().HaveCount(2);
            var pic = Entry.ProfilePictures[0];

            pic.Url.Should().Be("https://hookuphotshot.com/wp-content/uploads/2020/03/BLK3527.jpg");

        }

        [Test]
        public void Crawler_Finds_DirtyPicture()
        {

            Entry.ProfilePictures.Should().HaveCount(2);
            var pic = Entry.ProfilePictures[1];

            pic.Url.Should().Be("https://hookuphotshot.com/wp-content/uploads/2020/03/BLK4145.jpg");

        }

    }
}
