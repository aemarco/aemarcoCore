using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class NudevistaTestsWithAmberSym : PersonCrawlerTestsBase<Nudevista>
{

    //https://www.nudevista.at/?q=Amber+Sym&s=s
    public NudevistaTestsWithAmberSym()
        : base("Amber Sym")
    {
        //first and last name expected automatically
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/319/145319_370.jpg");
        ExpectedAliases.AddRange(new List<string>
        {
            "Amber Symm",
            "Ohc Destiny",
            "Tara Marie",
            "Tara Marie Price",
            "Tara P."
        });
        ExpectedBirthday = new DateOnly(1989, 11, 4);
        ExpectedCountry = "USA";
        ExpectedProfession = "Fashion Model";
        ExpectedEthnicity = "Caucasian";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedMeasurementDetails = "86C-60-86";
        ExpectedHeight = 167;
        ExpectedWeight = 48;
        ExpectedPiercings = "Ears, Navel";
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedStillActive = false;
    }
}