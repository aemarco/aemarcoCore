namespace aemarco.Crawler.PersonTests.Common;
public class DataParserTests
{

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("Clara Fargo , Larissa", "Clara Fargo;Larissa")]
    [TestCase("Arizona, United States", "Arizona;United States")]
    [TestCase(
        "Angel C , Ekaterina D , Foxi Di , Katoa , Katya Ivanova , Kleine Punci , Medina U , Nensi B",
        "Angel C;Ekaterina D;Foxi Di;Katoa;Katya Ivanova;Kleine Punci;Medina U;Nensi B")]
    public void FindStringsInText_Works(string? text, string expected)
    {
        var result = DataParser.FindStringsInText(text);
        string.Join(';', result).Should().Be(expected);
    }

    [TestCase(null, null, null)]
    [TestCase("Chloe Temple", "Chloe", "Temple")]
    [TestCase("Foxy Di", "Foxy", "Di")]
    [TestCase("Ariel Rebel", "Ariel", "Rebel")]
    [TestCase("Aletta Ocean", "Aletta", "Ocean")]
    [TestCase("Amber Sym", "Amber", "Sym")]
    public void FindNameInText_Works(string? text, string? expectedFirst, string expectedLast)
    {
        var (firstName, lastName) = DataParser.FindNameInText(text);
        firstName.Should().Be(expectedFirst);
        lastName.Should().Be(expectedLast);
    }


    [TestCase(
        "/pics/Chloe%20Temple.jpg",
        "https://www.babepedia.com/",
        "https://www.babepedia.com/pics/Chloe%20Temple.jpg")]
    [TestCase(
        "https://www.babesandstars.com/models/2000/2094/250x330.jpg",
        "https://www.babesandstars.com/",
        "https://www.babesandstars.com/models/2000/2094/250x330.jpg")]
    [TestCase(
        "https://cdn.pornsites.xxx/models/6394/aletta-ocean-4.jpg",
        "https://cdn.pornsites.xxx/",
        "https://cdn.pornsites.xxx/models/6394/aletta-ocean-4.jpg")]
    [TestCase(
        "/ImgFiles/Ariel Rebel/1.jpg",
        "https://pornstarbyface.com/",
        "https://pornstarbyface.com/ImgFiles/Ariel%20Rebel/1.jpg")]
    [TestCase(
        "http://www.istripper.com/free/sets/a0822/illustrations/full.png",
        "https://www.istripper.com/",
        "http://www.istripper.com/free/sets/a0822/illustrations/full.png")]
    public void FindPictureUri_Works(string text, string site, string? expected)
    {
        var result = DataParser.FindPictureUri(text, new Uri(site));
        result.AbsoluteUri.Should().Be(expected);
    }

    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("Born: Sunday 6Th Of February 2000", "06.02.2000")]
    [TestCase("14 March 1988", "14.03.1988")]
    [TestCase("November 4, 1989", "04.11.1989")]
    [TestCase("Age:14.Dec.1987 (35)", "14.12.1987")]
    [TestCase("Birthday: 14 Mar 1988", "14.03.1988")]
    public void FindBirthdayInText_Works(string? text, string? asText)
    {
        DateOnly? expected = asText is null ? null : DateOnly.Parse(asText);
        var result = DataParser.FindBirthdayInText(text);
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
        var result = DataParser.FindGenderInText(text);
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
        var result = DataParser.FindCareerStartInText(text);
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
    public void FindHeightInText_Works(string? text, int? expected)
    {
        var result = DataParser.FindHeightInText(text);
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
        var result = DataParser.FindWeightInText(text);
        result.Should().Be(expected);
    }


    [TestCase(null, null)]
    [TestCase("nothing to find", null)]
    [TestCase("Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)", true)]
    [TestCase("Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)", false)]
    [TestCase("Karrierestatus: Retired", false)]
    public void FindStillActiveInText_Works(string? text, bool? expected)
    {
        var result = DataParser.FindStillActiveInText(text);
        result.Should().Be(expected);
    }

    [TestCase(null, false, "")]
    [TestCase("Cup: A", false, "A")]
    [TestCase("32B", false, "B")]
    [TestCase("Körbchengröße: D (Fake)", false, "D")]
    [TestCase("Maße: 86 / 66 / 94", false, "86-66-94")]
    [TestCase("34A-23-32", true, "86A-58-81")]
    [TestCase("Measurements: 32B-24-35", true, "81B-60-88")]
    [TestCase("Measurements: 34-24-34", true, "86-60-86")]
    [TestCase("Maße: 38-26-37", true, "96-66-93")]
    [TestCase("Cupsize:38DDD-26-37", true, "96DDD-66-93")]
    public void FindMeasurementDetailsFromText_Works(string? text, bool isInches, string expected)
    {
        var result = DataParser.FindMeasurementDetailsFromText(text, isInches);
        result.ToString().Should().Be(expected);
    }




}
