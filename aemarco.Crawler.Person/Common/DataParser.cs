﻿namespace aemarco.Crawler.Person.Common;

public static class DataParser
{

    /// <summary>
    /// Splits given text into trimmed chunks with no empty entries
    /// </summary>
    /// <param name="text"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static List<string> FindStringsInText(string? text, char separator = ',')
    {
        if (text is null)
            return new List<string>();

        var split = text.Split(
            separator,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return split.ToList();
    }

    /// <summary>
    /// Tries to parse out FirstName and LastName
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static (string? FirstName, string? LastName) FindNameInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || !text.Contains(' '))
            return (null, null);

        text = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        var split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return split.Length == 2
                ? (split[0], split[1])
                : (null, null);
    }

    /// <summary>
    /// Forms a absolute Uri from given href
    /// </summary>
    /// <param name="imageRef">href</param>
    /// <param name="siteUri">site uri</param>
    /// <returns></returns>
    public static Uri FindPictureUri(string imageRef, Uri siteUri)
    {
        var result = Uri.TryCreate(imageRef, UriKind.Absolute, out var absUri)
            ? absUri
            : new Uri(siteUri, imageRef);

        return result;
    }

    /// <summary>
    /// Tries to find the birthday in the text
    /// - Born: Sunday 6Th Of February 2000
    /// - 14 March 1988
    /// - Geburtstag: November 4, 1989
    /// - Age:14.Dec.1987 (35)
    /// Birthday: 14 Mar 1988
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static DateOnly? FindBirthdayInText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // handles:
        // Age:14.Dec.1987 (35)
        // Birthday: 14 Mar 1988
        var doted = Regex.Match(text, @"\d{1,2}\.*\s*(\w+)\.*\s*\d{4}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (doted.Success)
        {
            return DateOnly.Parse(doted.Groups[0].Value);
        }

        // handles:
        // Born: Sunday 6Th Of February 2000
        // Born: Wednesday 14Th Of September 1994
        // 14 March 1988
        var textual = Regex.Match(text, @"(\d+)\s*(?:th)?\s*(?:of)?\s*(\w+)\s(\d{4,4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (textual.Success)
        {
            // 6 February 2000
            var shorter = $"{textual.Groups[1]} {textual.Groups[2]} {textual.Groups[3]}";
            return DateOnly.ParseExact(shorter, "d MMMM yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces);
        }

        // handles:
        // Geburtstag: December 14, 1987
        // Geburtstag: November 4, 1989
        var strange = Regex.Match(text, @"(\w+)\s(\d{1,2}),\s(\d{4,4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (strange.Success)
        {
            var shorter = $"{strange.Groups[1]} {strange.Groups[2]} {strange.Groups[3]}";
            return DateOnly.ParseExact(shorter, "MMMM d yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces);
        }
        return null;
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

        var years = Regex.Match(text, @"(\d{4,4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (years.Success &&
            int.TryParse(years.Groups[1].Value, out var year))
        {
            return new DateOnly(year, 1, 1);
        }

        return null;
    }

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

        var cms = Regex.Match(text, @"(\d+)[ ]?cm", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (cms.Success && int.TryParse(cms.Groups[1].Value, out var cm))
        {
            return cm;
        }

        var feetInch = Regex.Match(text, @"(\d+)(?:'|′)[ ]?(\d+)(?:""|''|″)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (feetInch.Success &&
            int.TryParse(feetInch.Groups[1].Value, out var feet) &&
            int.TryParse(feetInch.Groups[2].Value, out var inch))
        {
            return Convert.ToInt32((feet * 12 + inch) * 2.54);
        }

        var feetOnly = Regex.Match(text, @"(\d+)'", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (feetOnly.Success && int.TryParse(feetOnly.Groups[1].Value, out var ft))
        {
            return Convert.ToInt32(ft * 12 * 2.54);
        }

        return null;
    }

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

        var kilo = Regex.Match(text, @"(\d+)[ ]?kg", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (kilo.Success && int.TryParse(kilo.Groups[1].Value, out var kilos))
        {
            return kilos;
        }

        var libs = Regex.Match(text, @"(\d+)[ ]?lbs", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (libs.Success && int.TryParse(libs.Groups[1].Value, out var lbs))
        {
            return (int)(1.0 * lbs / 2.20462);
        }
        return null;
    }

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

        if (Regex.IsMatch(text, "-[ ]?active", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(text, "-[ ]?present", RegexOptions.IgnoreCase))
            return true;


        if (Regex.IsMatch(text, "retired", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(text, @"-[ ]?\d{4,4}")) // - 2015
            return false;

        return null;
    }

    /// <summary>
    /// Tries to get Measurement details with cup size
    /// - Cup: A
    /// - 32B
    /// - Körbchengröße: D (Fake)
    /// - 34A-23-32
    /// - Measurements: 32B-24-35
    /// - Measurements: 34-24-34
    /// - Maße: 86 / 66 / 94
    /// - Maße: 38-26-37
    /// - Cupsize:38DDD-26-37
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isInches"></param>
    public static MeasurementDetails FindMeasurementDetailsFromText(string? text, bool isInches = false)
    {
        var result = MeasurementDetails.Empty;
        if (string.IsNullOrWhiteSpace(text))
            return result;

        text = text
            .Replace(" / ", "-")
            .Replace("Measurements:", string.Empty)
            .Replace("Maße:", string.Empty)
            .Replace("Cupsize:", string.Empty)
            .Replace("Cup:", string.Empty)
            .Replace("Körbchengröße:", string.Empty);


        var measures = Regex.Match(text, @"(\d+)([a-dA-D]*)-(\d+)-(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (measures.Success &&
            int.TryParse(measures.Groups[1].Value, out var bust) &&
            measures.Groups[2].Value is { } cup &&
            int.TryParse(measures.Groups[3].Value, out var waist) &&
            int.TryParse(measures.Groups[4].Value, out var hip))
        {
            if (isInches)
            {
                bust = (int)(bust * 2.54);
                waist = (int)(waist * 2.54);
                hip = (int)(hip * 2.54);
            }

            cup = string.IsNullOrWhiteSpace(cup)
                ? null
                : cup.Trim();
            result = result.Combine(new MeasurementDetails(bust, cup, waist, hip));
        }

        var cupMatch = Regex.Match(text, @"[A-D]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (cupMatch.Success)
        {
            result = result.Combine(new MeasurementDetails(null, cupMatch.Value, null, null));
        }

        return result;
    }

}