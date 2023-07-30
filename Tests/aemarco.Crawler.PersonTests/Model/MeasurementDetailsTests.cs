namespace aemarco.Crawler.PersonTests.Model;
internal class MeasurementDetailsTests
{

    [TestCase(null, null, null, null, "")]
    [TestCase(null, "B", 31, 32, "B")]
    [TestCase(30, "B", null, 32, "B")]
    [TestCase(30, "B", 31, null, "B")]
    [TestCase(30, null, 31, 32, "30-31-32")]
    [TestCase(30, "C", 31, 32, "30C-31-32")]
    [TestCase(30, "c", 31, 32, "30C-31-32")]
    public void ToString_Works(int? bust, string? cup, int? waist, int? hip, string expected)
    {
        var sut = new MeasurementDetails(bust, cup, false, waist, hip);

        var result = sut.ToString();

        result.Should().Be(expected);
    }



    [TestCase("", "")]
    [TestCase("A", "A")]
    [TestCase("D (Fake)", "D(fake)")]
    [TestCase("32A", "A")]
    [TestCase("86 / 66 / 94", "86-66-94")]
    [TestCase("86-60-87", "86-60-87")]
    [TestCase("86A / 66 / 94", "86A-66-94")]
    [TestCase("86A-58-81", "86A-58-81")]
    [TestCase("96DDD/66/93", "96F-66-93")]
    [TestCase("96DDD-66-93", "96F-66-93")]
    [TestCase("86DD(fake)/66/93", "86E(fake)-66-93")]
    [TestCase("86DD(fake)-66-93", "86E(fake)-66-93")]
    public void Parse_Works(string text, string expected)
    {
        var result = MeasurementDetails.Parse(text);
        result.ToString().Should().Be(expected);
    }


    [TestCase("", "")]
    [TestCase("A", "A")]
    [TestCase("D", "D")]
    [TestCase("DD", "E")]
    [TestCase("DDD", "F")]
    [TestCase("DDDD", "G")]
    [TestCase("DDDDD", "H")]
    [TestCase("32A", "A")]
    [TestCase("34A / 23 / 32", "86A-58-81")]
    [TestCase("34A/23/32", "86A-58-81")]
    [TestCase("34A-23-32", "86A-58-81")]
    [TestCase("32B-24-35", "81B-60-88")]
    [TestCase("34-24-34", "86-60-86")]
    [TestCase("38-26-37", "96-66-93")]
    [TestCase("38DDD/26/37", "96F-66-93")]
    [TestCase("38D-26-37", "96D-66-93")]
    [TestCase("38DD-26-37", "96E-66-93")]
    [TestCase("38DDD-26-37", "96F-66-93")]
    [TestCase("38DDDD-26-37", "96G-66-93")]
    [TestCase("38DDDDD-26-37", "96H-66-93")]
    public void Parse_WorksForImperial(string text, string expected)
    {
        var format = new MeasurementFormatProvider(MeasurementSystem.Imperial);
        var result = MeasurementDetails.Parse(text, format);

        result.ToString().Should().Be(expected);
    }




    [TestCase(null)]
    [TestCase("ZZZ")]
    public void TryParse_WorksUnSuccessfully(string? text)
    {
        var result = MeasurementDetails.TryParse(text, out var parsed);
        result.Should().BeFalse();
        parsed.Bust.Should().Be(null);
        parsed.Cup.Should().Be(null);
        parsed.Waist.Should().Be(null);
        parsed.Hip.Should().Be(null);
    }

}
