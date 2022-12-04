﻿using aemarco.Crawler.Person.Crawlers;
using aemarco.Crawler.PersonTests.Base;
using System;
using System.Collections.Generic;

namespace aemarco.Crawler.PersonTests.Crawlers;

internal class PersonCrawlerNudevistaTestsWithAmberSym : PersonCrawlerTestsBase<Nudevista>
{

    //https://www.nudevista.at/?q=Amber+Sym&s=s
    public PersonCrawlerNudevistaTestsWithAmberSym()
        : base("Amber Sym")
    {
        ExpectedProfilePictures.Add("https://b99.nudevista.com/_/319/145319_370.jpg");
        ExpectedBirthday = new DateTime(1989, 11, 4, 0, 0, 0, DateTimeKind.Utc);
        ExpectedCountry = "USA";
        ExpectedProfession = "Fashion Model";
        ExpectedCareerStart = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
        ExpectedCupsize = "C (fake)";
        ExpectedHeight = 167;
        ExpectedWeight = 48;
        ExpectedPiercings = "Ears, Navel";
    }
}