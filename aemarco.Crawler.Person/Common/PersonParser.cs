namespace aemarco.Crawler.Person.Common;

public static partial class PersonParser
{

    /// <summary>
    /// Tries to parse out FirstName and LastName
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static (string? FirstName, string? LastName) FindNameInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || !text.Contains(' '))
            return (null, null);

        text = text.ToLower().TitleCase();
        var split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return split.Length == 2
                ? (split[0], split[1])
                : (null, null);
    }

    /// <summary>
    /// Parses a double 0 - 10 rating out of a text. Uses . or , as separator
    /// </summary>
    /// <param name="text"></param>
    /// <param name="hundredBased"> if its based on 0 - 100</param>
    /// <returns></returns>
    public static double? FindRatingInText(string? text, bool hundredBased = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        //percent values to be / by 10 to get in range
        if (text.EndsWith("%"))
        {
            if (!int.TryParse(text.Except("%"), out var percent))
                return null;
            var maybeInRange = 1.0 * percent / 10;
            return maybeInRange is >= 0 and <= 10
                ? maybeInRange
                : null;
        }

        text = text.Replace(',', '.');
        if (!double.TryParse(text, NumberFormatInfo.InvariantInfo, out var rating))
        {
            return null;
        }

        if (hundredBased) //maybe hundred based
        {
            rating = 1.0 * rating / 10;
        }
        return rating is >= 0 and <= 10
            ? rating
            : null;
    }

    /// <summary>
    /// Tries to find gender from given text
    /// </summary>
    /// <param name="text"></param>
    public static Gender? FindGenderInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        KeyValuePair<string, Gender>[] genderMatches =
        {
            new("Female", Gender.Female),
            new("Male", Gender.Male),
            new("Trans", Gender.Other),
            new("tranny", Gender.Other)
        };
        var result = genderMatches
            .Where(pair => Regex.IsMatch(text, pair.Key, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            .Select(pair => (Gender?)pair.Value)
            .FirstOrDefault();
        return result;
    }

    [GeneratedRegex(@"(\d{4,4})", RegexOptions.IgnoreCase)]
    private static partial Regex YearRegex();

    /// <summary>
    /// Tries to find the year where the career has been started
    /// - Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)
    /// - Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)
    /// - Karrierestart: 2007
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static DateOnly? FindCareerStartInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (YearRegex().Match(text) is { Success: true } yearMatch &&
            int.TryParse(yearMatch.Groups[1].Value, out var year))
        {
            return new DateOnly(year, 1, 1);
        }

        return null;
    }

    [GeneratedRegex(@"(\d+)[ ]?cm", RegexOptions.IgnoreCase)]
    private static partial Regex CmRegex();

    [GeneratedRegex(@"(\d+)(?:'|′)[ ]?(\d+)(?:""|''|″)", RegexOptions.IgnoreCase)]
    private static partial Regex FeetInchRegex();

    [GeneratedRegex(@"(\d+)'", RegexOptions.IgnoreCase)]
    private static partial Regex FeetRegex();

    /// <summary>
    /// Tries to find the height in cm from text
    /// - 5'2''
    /// - Height: 5'3" (Or 160 Cm)
    /// - Height: 5'2" (Or 157 Cm)
    /// - Größe 178 Cm
    /// - Größe: 5′8″ (172 Cm)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static int? FindHeightInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;


        if (CmRegex().Match(text) is { Success: true } cmMatch
            && int.TryParse(cmMatch.Groups[1].Value, out var cm))
        {
            return cm;
        }

        if (FeetInchRegex().Match(text) is { Success: true } feetInchMatch &&
            int.TryParse(feetInchMatch.Groups[1].Value, out var feet) &&
            int.TryParse(feetInchMatch.Groups[2].Value, out var inch))
        {
            return Convert.ToInt32((feet * 12 + inch) * 2.54);
        }

        if (FeetRegex().Match(text) is { Success: true } feetMatch &&
            int.TryParse(feetMatch.Groups[1].Value, out var ft))
        {
            return Convert.ToInt32(ft * 12 * 2.54);
        }

        return null;
    }

    [GeneratedRegex(@"(\d+)[ ]?kg", RegexOptions.IgnoreCase)]
    private static partial Regex KgRegex();

    [GeneratedRegex(@"(\d+)[ ]?lbs", RegexOptions.IgnoreCase)]
    private static partial Regex LbsRegex();

    /// <summary>
    /// Find the kg value in a text
    /// - 99 Lbs
    /// - Weight: 101 Lbs (Or 46 Kg)
    /// - Weight: 99 Lbs (Or 45 Kg)
    /// - Gewicht: 57 Kg
    /// - Gewicht: 130 (59 Kg)
    /// - Weight:128 Lbs (58 Kg)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static int? FindWeightInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (KgRegex().Match(text) is { Success: true } kgMatch &&
            int.TryParse(kgMatch.Groups[1].Value, out var kilos))
        {
            return kilos;
        }


        if (LbsRegex().Match(text) is { Success: true } lbsMatch &&
            int.TryParse(lbsMatch.Groups[1].Value, out var lbs))
        {
            return (int)(1.0 * lbs / 2.20462);
        }

        return null;
    }

    [GeneratedRegex(@"(active|present)", RegexOptions.IgnoreCase)]
    private static partial Regex ActiveRegex();

    [GeneratedRegex(@"-[ ]?\d{4,4}", RegexOptions.IgnoreCase)]
    private static partial Regex UntilYearRegex(); //z.B. "- 2015"


    /// <summary>
    /// Tries to extract the info, if the person is still active
    /// - Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)
    /// - Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)
    /// - Karrierestatus: Retired
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool? FindStillActiveInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (ActiveRegex().IsMatch(text))
            return true;

        if (text.Contains("retired", StringComparison.OrdinalIgnoreCase) ||
            UntilYearRegex().IsMatch(text)) // - 2015
            return false;

        return null;
    }

}