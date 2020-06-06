using aemarcoCore.Crawlers.Crawlers;
using aemarcoCore.Crawlers.Types;
using NUnit.Framework;
using System.Threading;
using FluentAssertions;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPornpicsTests
    {

        PersonEntry _entry;

        [OneTimeSetUp]
        public void Setup()
        {
            _entry = new PersonCrawlerPornpics("Aletta Ocean", CancellationToken.None).GetPersonEntry();

        }

        [Test]
        public void Crawler_Finds_Girl()
        {
            Assert.IsTrue(_entry.IsValid);
        }

        [Test]
        public void Crawler_Finds_FirstName()
        {
            Assert.AreEqual("Aletta", _entry.FirstName);
        }

        [Test]
        public void Crawler_Finds_LastName()
        {
            Assert.AreEqual("Ocean", _entry.LastName);
        }

        [Test]
        public void Crawler_Finds_Picture()
        {

            _entry.ProfilePictures.Should().HaveCount(1);

           
        }


    }
}
