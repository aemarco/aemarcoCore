using aemarcoCommons.PersonCrawler.Crawlers;
using aemarcoCommons.PersonCrawlerTests.Base;
using System;

namespace aemarcoCommons.PersonCrawlerTests.Crawlers
{
    internal class PersonCrawlerPorngathererTestsAlettaOcean : PersonCrawlerTestsBase<PersonCrawlerPorngatherer>
    {
        public PersonCrawlerPorngathererTestsAlettaOcean()
            : base("Aletta Ocean")
        {
            ExpectedBirthday = new DateTime(1987, 12, 14, 0, 0, 0, DateTimeKind.Utc);
            ExpectedHeight = 172;
            ExpectedWeight = 58;
            ExpectedProfilePictures.Add("https://cdn.pornsites.xxx/models/profiles/Aletta-Ocean-6394_13.jpg");
        }
    }
}
