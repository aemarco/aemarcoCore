using System.Globalization;

namespace aemarco.Crawler.Person.Common;

internal abstract class PersonCrawlerBase
{

    private readonly Uri _siteUrl;
    internal PersonCrawlerBase(string nameToCrawl, Uri siteUrl)
    {
        NameToCrawl = nameToCrawl;
        _siteUrl = siteUrl;
        Result = new PersonInfo(this);
    }
    internal string NameToCrawl { get; }
    protected PersonInfo Result { get; }


    internal async Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
    {
        var href = GetSiteHref();
        var target = new Uri(_siteUrl, href);
        var document = await HtmlHelper.GetHtmlDocument(target);
        cancellationToken.ThrowIfCancellationRequested();

        await HandleDocument(document, cancellationToken);
        return Result;
    }

    protected abstract string GetSiteHref();
    protected abstract Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken);


    #region name

    protected void AddNameFromInnerText(HtmlNode? node)
    {
        if (node is null)
            return;

        var innerText = GetInnerText(node);
        AddName(innerText);
    }
    protected void AddName(string name)
    {
        if (!name.Contains(' '))
            return;

        name = Thread.CurrentThread.CurrentCulture.TextInfo
            .ToTitleCase(name.ToLower());

        var split = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (split.Length < 2)
            return;

        Result.FirstName = split[0];
        Result.LastName = split[1];
    }

    #endregion

    #region pictures

    protected void AddProfilePictures(HtmlNodeCollection? nodes, string attributeName, Func<string, Uri>? toUrl = null, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (nodes is null)
            return;
        foreach (var node in nodes)
        {
            AddProfilePicture(node, attributeName, toUrl, suggestedMinAdultLevel, suggestedMaxAdultLevel);
        }
    }
    protected void AddProfilePicture(HtmlNode? node, string attributeName, Func<string, Uri>? toUrl = null, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (node?.Attributes[attributeName]?.Value is not { } imageRef)
            return;

        var url = toUrl is null
            ? imageRef
            : toUrl(imageRef).AbsoluteUri;
        AddProfilePicture(url, suggestedMinAdultLevel, suggestedMaxAdultLevel);
    }
    protected Uri UrlFromHref(string imageRef)
    {
        return new Uri(_siteUrl, imageRef);
    }

    protected void AddProfilePicture(Uri site, string href, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        var uri = new Uri(site, href);
        AddProfilePicture(uri.AbsoluteUri, suggestedMinAdultLevel, suggestedMaxAdultLevel);
    }
    protected void AddProfilePicture(string url, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        var uri = new Uri(WebUtility.HtmlDecode(url));
        var profilePicture = new ProfilePicture(uri.AbsoluteUri, suggestedMinAdultLevel, suggestedMaxAdultLevel);
        Result.ProfilePictures.Add(profilePicture);
    }

    #endregion

    #region Details

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
    protected DateOnly? FindBirthdayInText(string text)
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
    /// Tries to find the height in cm from text
    /// - 5'2''
    /// - Height: 5'3" (Or 160 Cm)
    /// - Height: 5'2" (Or 157 Cm)
    /// - Größe 178 Cm
    /// - Größe: 5′8″ (172 Cm)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected int? FindHeightInText(string? text)
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
    protected int? FindWeightInText(string? text)
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
    /// Tries to update MeasurementDetails from text
    /// - 34A-23-32
    /// - Measurements: 32B-24-35
    /// - Measurements: 34-24-34
    /// - Maße: 86 / 66 / 94
    /// - Maße: 38-26-37
    /// - Cupsize:38DDD-26-37
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isInches"></param>
    protected void UpdateFromMeasurementsText(string? text, bool isInches = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        text = text.Replace(" / ", "-");

        var measures = Regex.Match(text, @"(\d+)([a-zA-Z]*)-(\d+)-(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

            Result.MeasurementDetails ??= new MeasurementDetails();
            Result.MeasurementDetails.Bust = bust;
            Result.MeasurementDetails.Waist = waist;
            Result.MeasurementDetails.Hip = hip;
            //this cup has priority
            if (!string.IsNullOrWhiteSpace(cup))
                Result.MeasurementDetails.Cup = cup;
        }
    }

    /// <summary>
    /// Tries to update Measurement details with cup size
    /// - A
    /// - 32B
    /// - D (Fake)
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateFromCupText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var cupMatch = Regex.Match(text, @"[A-D]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (cupMatch.Success)
        {
            Result.MeasurementDetails ??= new MeasurementDetails();
            Result.MeasurementDetails.Cup ??= cupMatch.Value;
        }
    }

    /// <summary>
    /// Tries to find the year where the career has been started
    /// - Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)
    /// - Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)
    /// - Karrierestart: 2007
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected DateOnly? FindCareerStartInText(string? text)
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
    /// Tries to extract the info, if the person is still active
    /// - Years Active: 2018 - Present (Started Around 18 Years Old; 4 Years In The Business)
    /// - Years Active: 2013 - 2019 (Started Around 18 Years Old; 6 Years In The Business)
    /// - Karrierestatus: Retired
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected bool? FindStillActiveInText(string? text)
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

    #endregion

    #region helper

    protected string GetInnerText(HtmlNode node, bool decode = true, bool trim = true, bool titleCase = true, params string[] removals)
    {
        var str = node.InnerText;
        if (decode)
            str = WebUtility.HtmlDecode(str);
        foreach (var removal in removals)
            str = str.Replace(removal, string.Empty, StringComparison.OrdinalIgnoreCase);
        if (trim)
            str = str.Trim();
        if (titleCase)
            str = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
        return str;
    }

    protected List<string> GetListFromCsv(string text, char separator)
    {
        var split = text.Split(
            separator,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return split.ToList();
    }

    #endregion

}