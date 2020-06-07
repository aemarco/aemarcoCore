using aemarcoCore.Crawlers.Crawlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace aemarcoCoreTests.CrawlersTests.CrawlersTests
{
    public class PersonCrawlerNudevistaTestsWithAmberSym : PersonCrawlerTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupEntry(new PersonCrawlerNudevista("Amber Sym", CancellationToken.None));

            ExpectedProfilePictures.Add("https://b99.nudevista.com/_/319/145319_370.jpg");
            ExpectedBirthday = new DateTime(1989, 11, 4, 0, 0, 0, DateTimeKind.Utc);
            ExpectedCountry = "USA";
            ExpectedProfession = "Fashion Model";
            ExpectedCareerStart = new DateTime(2013,1,1,0,0,0, DateTimeKind.Utc);
            ExpectedStillActive = false;
            ExpectedAliases.AddRange(new List<string>
            {
                "Amber Symm",
                "Ohc Destiny",
                "Tara Marie",
                "Tara Marie Price",
                "Tara P."
            });
            ExpectedEthnicity = "Caucasian";
            ExpectedHairColor = "Brown";
            ExpectedEyeColor = "Brown";
            ExpectedMeasurements = "86-60-86";
            ExpectedCupsize = "C (natural)";
            ExpectedHeight = 167;
            ExpectedWeight = 48;
            ExpectedPiercings = "Ears, Navel";


        }
    }
}
