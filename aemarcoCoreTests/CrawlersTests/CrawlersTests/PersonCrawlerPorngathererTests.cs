using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerPorngathererTests : PersonCrawlerTestsBase
    {

        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerPorngatherer("Aletta Ocean", CancellationToken.None));

            ExpectedBirthday = new DateTime(1987,12,14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedHeight = 172;
            ExpectedWeight = 58;
            ExpectedProfilePictures.Add("https://cdn.pornsites.xxx/models/profiles/Aletta-Ocean-6394_4.jpg");
        }
    }
}
