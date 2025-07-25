namespace aemarco.Crawler.PersonTests.Common;
public class PersonParserTests
{

    [TestCase(null, null, null)]
    [TestCase("Chloe Temple", "Chloe", "Temple")]
    [TestCase("Foxy Di", "Foxy", "Di")]
    [TestCase("Ariel Rebel", "Ariel", "Rebel")]
    [TestCase("Aletta Ocean", "Aletta", "Ocean")]
    [TestCase("Amber Sym", "Amber", "Sym")]
    public void FindNameInText_Works(string? text, string? expectedFirst, string? expectedLast)
    {
        var (firstName, lastName) = PersonParser.FindNameInText(text);
        firstName.Should().Be(expectedFirst);
        lastName.Should().Be(expectedLast);
    }


    [TestCase(null, false, null)]
    [TestCase("", false, null)]
    [TestCase("Bob", false, null)]
    [TestCase("6.4", false, 6.4)]
    [TestCase("6,4", false, 6.4)]
    [TestCase("6", false, 6)]
    [TestCase("-0.1", false, null)]
    [TestCase("-1", false, null)]
    [TestCase("10.1", false, null)]
    [TestCase("11", false, null)]
    [TestCase("11", true, 1.1)]
    [TestCase("55", true, 5.5)]
    [TestCase("97%", false, 9.7)]
    public void FindRatingInText_Works(string? text, bool hundredBased, double? expected)
    {
        var result = PersonParser.FindRatingInText(text, hundredBased);
        result.Should().Be(expected);
    }


    [TestCase(null, null)]
    [TestCase("Female", "Female")]
    [TestCase("Male", "Male")]
    [TestCase("Trans", "Other")]
    [TestCase("tranny", "Other")]
    public void FindGenderInText_Works(string? text, string? asText)
    {
        Gender? expected = asText is null ? null : Enum.Parse<Gender>(asText);
        var result = PersonParser.FindGenderInText(text);
        result.Should().Be(expected);
    }

    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)", "01.01.2018")]
    [TestCase("Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)", "01.01.2013")]
    [TestCase("Karrierestart: 2007", "01.01.2007")]
    public void FindCareerStartInText_Works(string? text, string? asText)
    {
        DateOnly? expected = asText is null ? null : DateOnly.Parse(asText);
        var result = PersonParser.FindCareerStartInText(text);
        result.Should().Be(expected);
    }

    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("5'2''", 157)]
    [TestCase("5'", 152)]
    [TestCase("Height: 5'3\" (Or 160 Cm)", 160)]
    [TestCase("Height: 5'2\" (Or 157 Cm)", 157)]
    [TestCase("Größe 178 Cm", 178)]
    [TestCase("Größe: 5′8″ (172 Cm)", 172)]
    [TestCase("5′8", 173)]
    public void FindHeightInText_Works(string? text, int? expected)
    {
        var result = PersonParser.FindHeightInText(text);
        result.Should().Be(expected);
    }

    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("99 Lbs", 44)]
    [TestCase("Weight: 101 Lbs (Or 46 Kg)", 46)]
    [TestCase("Weight: 99 Lbs (Or 45 Kg)", 45)]
    [TestCase("Gewicht: 57 Kg", 57)]
    [TestCase("Gewicht: 130 (59 Kg)", 59)]
    [TestCase("Weight:128 Lbs (58 Kg)", 58)]
    public void FindWeightInText_Works(string? text, int? expected)
    {
        var result = PersonParser.FindWeightInText(text);
        result.Should().Be(expected);
    }


    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("2018 - Present (Started Around 18 Years Old; 4 Years In The Business)", true)]
    [TestCase("2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)", false)]
    [TestCase("2013-2022 (Started Around 18 Years Old; 9 Years Active)", false)]
    [TestCase("Retired", false)]
    public void FindStillActiveInText_Works(string? text, bool? expected)
    {
        var result = PersonParser.FindStillActiveInText(text);
        result.Should().Be(expected);
    }

}
