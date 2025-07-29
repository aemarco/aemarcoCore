using System.IO;
using System.Text.Json;

namespace aemarco.Crawler.Services;

public interface ICountryService
{
    string? FindCountry(string? text);
}

internal class CountryService : ICountryService
{

    public string? FindCountry(string? text)
    {
        if (text is null)
            return null;


        //by name
        foreach (var entry in GetData())
        {
            if (Regex.IsMatch(text, entry.Name, RegexOptions.IgnoreCase))
            {
                return entry.Name;
            }
        }

        //by alias
        foreach (var entry in GetData()
                     .Where(x => x.Aliases.Length > 0))
        {
            var pattern = string.Join('|', entry.Aliases);
            if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
            {
                return entry.Name;
            }
        }

        //by 3code or 2code
        foreach (var entry in GetData()
                     .Where(x => x.ThreeLetterIsoName is not null))
        {
            if (text.Equals(entry.ThreeLetterIsoName, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Name;
            }
            if (text.Equals(entry.TwoLetterIsoName, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Name;
            }
        }

        //by region
        foreach (var entry in GetData()
                     .Where(x => x.Regions.Length > 0))
        {
            var pattern = string.Join('|', entry.Regions.Select(x => x.Name));
            if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
            {
                return entry.Name;
            }
        }

        return null;
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
