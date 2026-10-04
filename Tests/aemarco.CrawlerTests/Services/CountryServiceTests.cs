﻿using aemarco.Crawler.Services;
using aemarco.TestBasics;

namespace aemarco.CrawlerTests.Services;
internal class CountryServiceTests
{

    [TestCase(null, null)]
    [TestCase("Canada", "Canada")]
    [TestCase("Hungary", "Hungary")]
    [TestCase("United States", "United States")]
    [TestCase("American", "United States")]
    [TestCase("USA", "United States")]
    [TestCase("US", "United States")]
    [TestCase("Russian Federation", "Russia")]
    [TestCase("Russia", "Russia")]
    [TestCase("Russian", "Russia")]
    //a name which is part of another name or word
    [TestCase("Romania", "Romania")]
    [TestCase("romania", "Romania")]
    [TestCase("România", "Romania")]
    [TestCase("Bucharest, Romania", "Romania")]
    [TestCase("(Romania)", "Romania")]
    [TestCase("Oman", "Oman")]
    [TestCase("Muscat, Oman", "Oman")]
    [TestCase("Nigeria", "Nigeria")]
    [TestCase("Niger", "Niger")]
    [TestCase("Somalia", "Somalia")]
    [TestCase("Mali", "Mali")]
    [TestCase("South Sudan", "South Sudan")]
    [TestCase("Sudan", "Sudan")]
    [TestCase("Papua New Guinea", "Papua New Guinea")]
    [TestCase("Equatorial Guinea", "Equatorial Guinea")]
    [TestCase("Guinea", "Guinea")]
    [TestCase("Dominican Republic", "Dominican Republic")]
    [TestCase("Dominica", "Dominica")]
    [TestCase("American Samoa", "American Samoa")]
    [TestCase("Samoa", "Samoa")]
    [TestCase("Czech Republic", "Czechia")]
    [TestCase("Born in Hungary", "Hungary")]
    [TestCase("Woman", null)]
    [TestCase("", null)]
    [TestCase("   ", null)]
    public void FindCountry(string? text, string? expected)
    {
        var result = new CountryService().FindCountry(text);
        result.Should().Be(expected);
        TestHelper.PrintPassed(result);
    }


    [Test]
    public void GetData_DeliversAtLeastOne()
    {
        Country[] result = [.. new CountryService().GetData()];
        result.Length.Should().BeGreaterThan(0);
        TestHelper.PrintPassed(result);
    }

}
