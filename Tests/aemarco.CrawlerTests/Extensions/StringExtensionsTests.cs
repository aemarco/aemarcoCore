using System;

namespace aemarco.CrawlerTests.Extensions;

public class StringExtensionsTests
{

    [TestCase("Hello World", "Hello", new[] { "World" })]
    [TestCase("This is a test", "is", new[] { "this", "a", "test" })]
    [TestCase("TestTestTest", "", new[] { "test" })]
    [TestCase("abcABC123", "", new[] { "abc", "ABC", "123" })]
    [TestCase("", "", new[] { "Hello", "World" })]
    public void Except_RemovesGivenStrings(string input, string expected, params string[] except)
    {
        var result = input.Except(except);
        result.Should().Be(expected);
    }



    [TestCase("hello", "Hello")]
    [TestCase("the quick brown fox", "The Quick Brown Fox")]
    [TestCase("john's book: chapter one", "John's Book: Chapter One")]
    [TestCase("", "")]
    public void TitleCase_ReturnsExpectedResult(string input, string expected)
    {
        var result = input.TitleCase();
        result.Should().Be(expected);
    }



    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("Hello", "Hello")]
    [TestCase("Value1,Value2,Value3,Value4", "Value1;Value2;Value3;Value4")]
    [TestCase("  Value1 ,  Value2  , Value3   ", "Value1;Value2;Value3")] //trimming
    [TestCase("Value1,,Value2,,Value3", "Value1;Value2;Value3")] //empty
    public void SplitList_WorksWithComma(string? text, string expected)
    {
        var result = text.SplitList();
        string.Join(';', result).Should().Be(expected);
    }


    [TestCase("Value1,Value2,Value3", "Value1;Value2;Value3")]
    [TestCase("Value1;Value2;Value3", "Value1;Value2;Value3")]
    [TestCase("Value1|Value2|Value3", "Value1;Value2;Value3")]
    [TestCase("Value1,Value2;Value3|Value4", "Value1;Value2;Value3;Value4")]
    public void SplitList_WorksWithMultiple(string input, string expected)
    {
        var result = input.SplitList(',', ';', '|');
        string.Join(';', result).Should().Be(expected);
    }

    [TestCase(null, null, null, null)]
    [TestCase("nothing to find", null, null, null)]
    [TestCase("Sunday 6Th Of February 2000", 2000, 2, 6)]
    [TestCase("14 March 1988", 1988, 3, 14)]
    [TestCase("November 4, 1989", 1989, 11, 4)]
    [TestCase("14.Dec.1987", 1987, 12, 14)]
    [TestCase("14 Mar 1988", 1988, 3, 14)]
    public void ToDateOnly_Works(string? text, int? year, int? month, int? day)
    {
        DateOnly? expected = !year.HasValue || !month.HasValue || !day.HasValue
            ? null
            : new DateOnly(year.Value, month.Value, day.Value);
        var result = text.ToDateOnly();
        result.Should().Be(expected);
    }

}