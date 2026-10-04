﻿using System.IO;
using System.Text.Json;

namespace aemarco.Crawler.Services;

public interface ICountryService
{
    string? FindCountry(string? text);
}

internal class CountryService : ICountryService
{

    /// <summary>
    /// Finds the country a text names. The whole text being a name, alias or ISO code wins; otherwise a name or
    /// alias standing as whole words in the text counts, the longest first, so "South Sudan" is not "Sudan" and
    /// "Romania" is not "Oman" (which it contains), and finally a region name as whole words.
    /// </summary>
    public string? FindCountry(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        text = text.Trim();
        var countries = GetData().ToArray();

        //the whole text is a name, alias or code
        foreach (var entry in countries)
        {
            if (text.Equals(entry.Name, StringComparison.OrdinalIgnoreCase) ||
                entry.Aliases.Any(x => text.Equals(x, StringComparison.OrdinalIgnoreCase)) ||
                text.Equals(entry.TwoLetterIsoName, StringComparison.OrdinalIgnoreCase) ||
                (entry.ThreeLetterIsoName is not null &&
                 text.Equals(entry.ThreeLetterIsoName, StringComparison.OrdinalIgnoreCase)))
                return entry.Name;
        }

        //a name or alias as whole words in the text, the longest wins
        var byName = countries
            .SelectMany(entry => entry.Aliases
                .Append(entry.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => (entry.Name, Word: x)))
            .Where(x => ContainsWord(text, x.Word))
            .OrderByDescending(x => x.Word.Length)
            .FirstOrDefault();
        if (byName.Name is not null)
            return byName.Name;

        //by region
        foreach (var entry in countries)
        {
            if (entry.Regions.Any(x => ContainsWord(text, x.Name)))
                return entry.Name;
        }

        return null;
    }

    /// <summary>
    /// True when the word stands in the text on its own, not as a part of a longer word
    /// ("oman" is not in "Romania" in this sense, but is in "Oman, Muscat").
    /// </summary>
    internal static bool ContainsWord(string text, string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        var start = 0;
        while (text.IndexOf(word, start, StringComparison.OrdinalIgnoreCase) is var index and >= 0)
        {
            var end = index + word.Length;
            if ((index == 0 || !char.IsLetterOrDigit(text[index - 1])) &&
                (end == text.Length || !char.IsLetterOrDigit(text[end])))
                return true;
            start = index + 1;
        }
        return false;
    }

    private Country[]? _countries;
    internal IEnumerable<Country> GetData()
    {
        if (_countries is not null)
            return _countries;

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("aemarco.Crawler.Resources.CountryRegionData.json")
                           ?? throw new Exception("Could not find \"aemarco.Crawler.Resources.CountryRegionData.json\"");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        if (string.IsNullOrWhiteSpace(json))
            throw new Exception("\"aemarco.Crawler.Resources.CountryRegionData.json\"\" looks empty");
        _countries = JsonSerializer.Deserialize<Country[]>(json) ?? [];
        return _countries;
    }
}

// ReSharper disable UnusedAutoPropertyAccessor.Global
public record Country
{

    /// <summary>
    /// english name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// contains aliases like native name
    /// </summary>
    public required string[] Aliases { get; init; }

    /// <summary>
    /// two-letter code defined in ISO 3166 for the country/region.
    /// </summary>
    public required string TwoLetterIsoName { get; init; }

    /// <summary>
    /// three-letter code defined in ISO 3166 for the country/region.
    /// </summary>
    public string? ThreeLetterIsoName { get; init; }


    public required Region[] Regions { get; init; }
}

public record Region
{
    public required string Name { get; init; }
}
// ReSharper restore UnusedAutoPropertyAccessor.Global
