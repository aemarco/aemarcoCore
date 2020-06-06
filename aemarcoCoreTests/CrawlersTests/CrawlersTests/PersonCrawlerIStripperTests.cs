using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System.Threading;
using FluentAssertions;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerIStripperTests : PersonCrawlerTestsBase
    {
        
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry( new PersonCrawlerIStripper("Aletta Ocean", CancellationToken.None));
        }

        [Test]
        public void Crawler_Finds_Picture()
        {

            Entry.ProfilePictures.Should().HaveCount(1);
            var pic = Entry.ProfilePictures[0];

            pic.Url.Should().Be("http://www.istripper.com/free/sets/a0822/illustrations/full.png");
            
        }

        [Test]
        public void Crawler_Finds_Land()
        {
            Assert.AreEqual("Hungary", Entry.Land);
        }

        [Test]
        public void Crawler_Finds_Geburtsort()
        {
            Assert.AreEqual("Budapest", Entry.Geburtsort);
        }

        [Test]
        public void Crawler_Finds_Größe()
        {
            Assert.AreEqual(178, Entry.Größe);
        }

        [Test]
        public void Crawler_Finds_Gewicht()
        {
            Assert.AreEqual(57, Entry.Gewicht);
        }

        [Test]
        public void Crawler_Finds_Maße()
        {
            Assert.AreEqual("86-66-94", Entry.Maße);
        }
    }
}
