namespace aemarco.Crawler.Extensions;

public static partial class StringExtensions
{

    /// <summary>
    /// Removes specified substrings from the input text, case-insensitively.
    /// </summary>
    /// <param name="text">The input text from which substrings will be removed.</param>
    /// <param name="except">An array of substrings to be removed from the input text.</param>
    /// <returns>The input text with the specified substrings removed.</returns>
    public static string Except(this string text, params string[] except)
    {

        foreach (var removal in except)
        {
            text = text.Replace(removal, string.Empty, StringComparison.OrdinalIgnoreCase);
        }
        return text.Trim();
    }


    /// <summary>
    /// Converts the input string to title case (also known as proper case),
    /// where the first letter of each word is capitalized, and the rest of the letters are lowercase.
    /// </summary>
    /// <param name="text">The input string to be converted to title case.</param>
    /// <returns>The input string in title case.</returns>
    public static string TitleCase(this string text) =>
        new CultureInfo("en-US").TextInfo.ToTitleCase(text);

    /// <summary>
    /// Splits a text to a list with given separators. Separator defaults to ','.
    /// Trims the entries and removes empty once.
    /// </summary>
    /// <param name="text">text to split</param>
    /// <param name="separators">separators to use</param>
    /// <returns></returns>
    public static IEnumerable<string> SplitList(this string? text, params char[] separators)
    {
        if (text is null)
            return [];
        separators = separators.Length == 0
            ? [',']
            : separators;

        var result = text
            .Split(separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        return result;
    }

    [GeneratedRegex(@"\d{1,2}\.*\s*(\w+)\.*\s*\d{4}", RegexOptions.IgnoreCase)]
    private static partial Regex DayMonthTextYearRegex();

    [GeneratedRegex(@"(\d+)\s*(?:th)?\s*(?:of)?\s*(\w+)\s(\d{4,4})", RegexOptions.IgnoreCase)]
    private static partial Regex DayTextDayMonthTextYearRegex();

    [GeneratedRegex(@"(\w+)\s(\d{1,2}),\s(\d{4,4})", RegexOptions.IgnoreCase)]
    private static partial Regex MonthTextDayYearRegex();

    /// <summary>
    /// Tries to extract a date from given text
    /// - Sunday 6Th Of February 2000
    /// - 14 March 1988
    /// - November 4, 1989
    /// - 14.Dec.1987
    /// - 14 Mar 1988
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static DateOnly? ToDateOnly(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // handles:
        // 14.Dec.1987
        // 14 Mar 1988
        if (DayMonthTextYearRegex().Match(text) is { Success: true } doted)
        {
            return DateOnly.Parse(doted.Groups[0].Value);
        }


        // handles:
        // Sunday 6Th Of February 2000
        // Wednesday 14Th Of September 1994
        // 14 March 1988
        if (DayTextDayMonthTextYearRegex().Match(text) is { Success: true } textual)
        {
            // 6 February 2000
            var shorter = $"{textual.Groups[1]} {textual.Groups[2]} {textual.Groups[3]}";
            return DateOnly.ParseExact(shorter, "d MMMM yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces);
        }


        // handles:
        // December 14, 1987
        // November 4, 1989
        if (MonthTextDayYearRegex().Match(text) is { Success: true } strange)
        {
            var shorter = $"{strange.Groups[1]} {strange.Groups[2]} {strange.Groups[3]}";
            return DateOnly.ParseExact(shorter, "MMMM d yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces);
        }
        return null;
    }

}