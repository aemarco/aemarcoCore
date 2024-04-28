using CountryData.Standard;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

// ReSharper disable SuggestBaseTypeForParameter
#nullable enable

class CountryRegionDataUpdater
{

    readonly Solution Solution;
    public CountryRegionDataUpdater(Solution solution)
    {
        Solution = solution;
    }

    readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public void UpdateCountryRegionData()
    {
        var proj = Solution.GetProject("aemarco.Crawler");
        proj = proj.NotNull("Could not find aemarco.Crawler");
        File.WriteAllText(
            proj.Directory / "Resources" / "CountryRegionData.json",
            GetJson());
    }

    string GetJson()
    {
        List<Country> data = [];
        FillFromCultureInfo(data);
        FillFromFrankData(data);
        FillFromChatGptData(data);
        data = [.. data
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.Name) &&
                !string.IsNullOrWhiteSpace(x.TwoLetterIsoName))
            .OrderBy(x => x.Name)];

        var result = JsonSerializer.Serialize(
            data,
            JsonOptions);
        return result;
    }

    #region CultureInfo

    //https://www.c-sharpcorner.com/UploadFile/0c1bb2/display-country-list-without-database-in-Asp-Net-C-Sharp/
    void FillFromCultureInfo(List<Country> data)
    {
        var infos = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Where(x => !x.CultureTypes.HasFlag(CultureTypes.UserCustomCulture))
            .Select(x => (x, new RegionInfo(x.LCID)));

        foreach (var (_, region) in infos)
        {
            if (data.FirstOrDefault(x => x.Name == region.EnglishName)
                is { } country)
            {

                if (!country.Aliases.Contains(region.NativeName))
                    country.Aliases.Add(region.NativeName);
            }
            else
            {//create
                country = new Country
                {
                    Name = region.EnglishName,
                    Aliases = [region.NativeName],
                    TwoLetterIsoName = region.TwoLetterISORegionName,
                    ThreeLetterIsoName = region.ThreeLetterISORegionName
                };
                data.Add(country);
            }
        }
    }


    #endregion

    #region FrankData

    //https://www.c-sharpcorner.com/blogs/country-data-for-c-sharp
    //https://github.com/frankodoom/CountryData.Net
    readonly CountryHelper FrankCountryHelper = new();
    void FillFromFrankData(List<Country> data)
    {
        foreach (var info in FrankCountryHelper.GetCountryData())
        {
            if (data.FirstOrDefault(x => x.TwoLetterIsoName == info.CountryShortCode)
                is { } country)
            {//merge
                if (country.Name != info.CountryName &&
                    !country.Aliases.Contains(info.CountryName))
                {
                    country.Aliases.Add(info.CountryName);
                }
            }
            else
            {//create
                country = new Country
                {
                    Name = info.CountryName,
                    TwoLetterIsoName = info.CountryShortCode
                };
                data.Add(country);
            }

            //merge in regions in any case
            foreach (var region in info.Regions
                         .Select(r => new Region
                         {
                             Name = r.Name
                         })
                         .Where(x => !country.Regions.Contains(x)))

            {
                country.Regions.Add(region);
            }
        }
    }

    #endregion

    #region ChatGPT

    readonly Dictionary<string, string[]> AdditionalAliases = new()
    {
        // ReSharper disable StringLiteralTypo
        { "Albania", ["Albanien"] },
        { "Algeria", ["Algerien"] },
        { "Antigua and Barbuda", ["Antigua und Barbuda"] },
        { "Argentina", ["Argentinien", "Argentinean"] },
        { "Armenia", ["Armenien"] },
        { "Australia", ["Australien"] },
        { "Azerbaijan", ["Aserbaidschan"] },
        { "Bangladesh", ["Bangladesch"] },
        { "Belarus", ["Weißrussland"] },
        { "Belgium", ["Belgien"] },
        { "Bolivia", ["Bolivien"] },
        { "Bosnia & Herzegovina", ["Bosnien und Herzegowina"] },
        { "Brazil", ["Brasilien"] },
        { "Bulgaria", ["Bulgarien"] },
        { "Cambodia", ["Kambodscha"] },
        { "Cameroon", ["Kamerun"] },
        { "Canada", ["Kanada", "Canadian"] },
        { "Central African Republic", ["Zentralafrikanische Republik"] },
        { "China", ["Chinese"] },
        { "Colombia", ["Kolumbien"] },
        { "Comoros", ["Comros", "Komoren"] },
        { "Congo (DRC)",["Congo, Democratic Republic of the", "Kongo, Demokratische Republik"] },
        { "Croatia", ["Kroatien"] },
        { "Cuba", ["Kuba"] },
        { "Cyprus", ["Zypern"] },
        { "Czechia", ["Tschechische Republik", "Czech"] },
        { "Denmark", ["Dänemark"] },
        { "Djibouti", ["Dschibuti"] },
        { "Dominican Republic", ["Dominikanische Republik"] },
        { "Ecuador", [  "Equador"] },
        { "Egypt", ["Ägypten"] },
        { "Equatorial Guinea", ["Äquatorialguinea"] },
        { "Estonia", ["Estland"] },
        { "Ethiopia", ["Äthiopien"] },
        { "Fiji", ["Fidschi"] },
        { "Finland", ["Finnland"] },
        { "France", ["Frankreich", "French"] },
        { "Gabon", ["Gabun"] },
        { "Gambia, The", ["Gambia"] },
        { "Georgia", ["Georgien"] },
        { "Greece", ["Griechenland", "Greek"] },
        { "Holy See (Vatican City)", ["Vatican City", "Vatikanstadt"] },
        { "Hungary", ["Ungarn", "Hungarian"] },
        { "Iceland", ["Island"] },
        { "India", ["Indien"] },
        { "Indonesia", ["Indonesien"] },
        { "Iraq", ["Irak"] },
        { "Ireland", ["Irland", "Irish"] },
        { "Italy", ["Italien", "Italian"] },
        { "Jamaica", ["Jamaika"] },
        { "Jordan", ["Jordanien"] },
        { "Kazakhstan", ["Kasachstan"] },
        { "Kenya", ["Kenia"] },
        { "Korea", ["South Korea", "Südkorea", "Korea, South", "South Korean"]},
        { "Korea, Democratic People's Republic of", ["North Korea", "Nordkorea", "Korea, North"]},
        { "Kyrgyzstan", ["Kirgisistan"] },
        { "Latvia", ["Lettland"] },
        { "Lebanon", ["Libanon"] },
        { "Libya", ["Libyen"] },
        { "Lithuania", ["Litauen"] },
        { "Madagascar", ["Madagaskar"] },
        { "Maldives", ["Malediven"] },
        { "Marshall Islands", ["Marshallinseln"] },
        { "Mauritania", ["Mauretanien"] },
        { "Mexico", ["Mexiko"] },
        { "Moldova", ["Moldawien"] },
        { "Morocco", ["Marokko"] },
        { "Mozambique", ["Mosambik"] },
        { "Netherlands", ["Niederlande"] },
        { "New Zealand", ["Neuseeland"] },
        { "North Macedonia", ["Nordmazedonien"] },
        { "Norway", ["Norwegen"] },
        { "Palestine, State of", ["Palestine", "Palästina"] },
        { "Papua New Guinea", ["Papua-Neuguinea"] },
        { "Philippines", ["Philippinen"] },
        { "Poland", ["Polen"] },
        { "Qatar", ["Katar"] },
        { "Romania", ["Rumänien"] },
        { "Russia", ["Russland", "Russian"] },
        { "Rwanda", ["Ruanda"] },
        { "Saint Kitts and Nevis", ["St. Kitts und Nevis"] },
        { "Saint Lucia", ["St. Lucia"] },
        { "Saint Vincent and the Grenadines", ["St. Vincent und die Grenadinen"] },
        { "Sao Tome and Principe", ["São Tomé und Príncipe"] },
        { "Saudi Arabia", ["Saudi-Arabien"] },
        { "Serbia", ["Serbien"] },
        { "Seychelles", ["Seychellen"] },
        { "Singapore", ["Singapur"] },
        { "Slovakia", ["Slowakei"] },
        { "Slovenia", ["Slowenien"] },
        { "Solomon Islands", ["Salomonen"] },
        { "South Africa", ["Südafrika"] },
        { "South Sudan", ["Südsudan"] },
        { "Spain", ["Spanien", "Spanish"] },
        { "Sweden", ["Schweden"] },
        { "Syria", ["Syrien"] },
        { "Tajikistan", ["Tadschikistan"] },
        { "Tanzania, United Republic of", ["Tanzania", "Tansania"] },
        { "Trinidad & Tobago", ["Trinidad und Tobago"] },
        { "Tunisia", ["Tunesien"] },
        { "Turkey", ["Türkei"] },
        { "Ukraine", [  "Ukrainian"] },
        { "United Arab Emirates", ["Vereinigte Arabische Emirate", "UAE"] },
        { "United Kingdom", ["Vereinigtes Königreich", "UK", "British"] },
        { "United States", ["Vereinigte Staaten", "American"] },
        { "Uzbekistan", ["Usbekistan"] },
        { "Yemen", ["Jemen"] },
        { "Zambia", ["Sambia"] },
        { "Zimbabwe", ["Simbabwe"] }
        // ReSharper restore StringLiteralTypo
    };


    void FillFromChatGptData(List<Country> data)
    {
        foreach (var (name, aliases) in AdditionalAliases)
        {
            if (data.FirstOrDefault(x => x.Name == name) is { } country)
            {
                string[] known = [
                    "",
                    country.Name,
                    country.ThreeLetterIsoName ?? string.Empty,
                    country.TwoLetterIsoName ?? string.Empty,
                    .. country.Aliases,
                    .. country.Regions.Select(r => r.Name)];

                foreach (var alias in aliases)
                {
                    if (!known.Contains(alias))
                        country.Aliases.Add(alias);
                    else
                        Log.Warning("Alias {alias} is already known for country {country}", alias, name);
                }
            }
            else
            {
                Log.Warning("Country {country} not found", name);
            }
        }
    }

    #endregion

}


// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

public record Country
{
    public required string Name { get; set; }
    public List<string> Aliases { get; set; } = [];
    public string? TwoLetterIsoName { get; set; }
    public string? ThreeLetterIsoName { get; set; }
    public List<Region> Regions { get; set; } = [];
}

public record Region
{
    public required string Name { get; set; }
}

// ReSharper restore PropertyCanBeMadeInitOnly.Global
// ReSharper restore UnusedAutoPropertyAccessor.Global

