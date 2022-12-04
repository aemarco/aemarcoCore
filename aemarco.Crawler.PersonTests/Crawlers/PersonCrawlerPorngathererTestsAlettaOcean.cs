using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PersonCrawlerPorngathererTestsAlettaOcean : PersonCrawlerTestsBase<Porngatherer>
{

    //https://pornsites.xxx/pornstars/Aletta-Ocean
    public PersonCrawlerPorngathererTestsAlettaOcean()
        : base("Aletta Ocean")
    {
        ExpectedBirthday = new DateTime(1987, 12, 14, 0, 0, 0, DateTimeKind.Utc);
        ExpectedHeight = 172;
        ExpectedWeight = 58;
        ExpectedProfilePictures.Add(
            "https://cdn.pornsites.xxx/models/6394/aletta-ocean-4.jpg");
    }
}