namespace aemarco.Crawler.Person.Model;

public class PersonInfo
{
    internal PersonInfo(PersonCrawlerBase crawler)
    {
        var info = crawler.GetType().ToCrawlerInfo();
        PersonEntrySource = info.FriendlyName;
        PersonEntryPriority = info.Priority;
    }

    internal string PersonEntrySource { get; }
    internal int PersonEntryPriority { get; }



    public string? FirstName { get; internal set; }
    public string? LastName { get; internal set; }
    public Gender? Gender { get; internal set; }
    public List<ProfilePicture> ProfilePictures { get; } = new();
    public DateOnly? Birthday { get; internal set; }
    public string? Country { get; internal set; }
    public string? City { get; internal set; }
    public string? Profession { get; internal set; }
    public DateOnly? CareerStart { get; internal set; }
    public bool? StillActive { get; internal set; }
    public List<string> Aliases { get; internal set; } = new();


    /// <example>
    ///Caucasian
    ///Latin
    ///Asian
    ///Black
    ///Mixed-race (primarily Latin)
    ///Mixed-race (primarily Asian)
    ///Ebony
    ///Mixed-race (primarily Caucasian)
    ///United States
    ///Middle Eastern
    ///Mixed-race (primarily Black)
    ///Indian
    ///Japanese
    ///Mixed-race
    ///Exotic
    ///American
    ///Euro
    ///Other
    ///Thai
    ///United Kingdom
    ///Russian Federation
    ///Czech Republic
    /// </example>
    public string? Ethnicity { get; internal set; }
    public string? HairColor { get; internal set; }
    public string? EyeColor { get; internal set; }
    public MeasurementDetails? MeasurementDetails { get; set; }
    public int? Height { get; internal set; }
    public int? Weight { get; internal set; }
    public string? Piercings { get; internal set; }

    internal void Merge(PersonInfo info)
    {
        FirstName ??= info.FirstName;
        LastName ??= info.LastName;

        if (info.FirstName != FirstName || info.LastName != LastName)
        {//not a first degree match

            if (!Aliases.Contains($"{info.FirstName} {info.LastName}") &&
                !info.Aliases.Contains($"{FirstName} {LastName}"))
            {//not a second degree match

                //we ignore this info, as it´s not matching our entry
                return;
            }
        }

        ProfilePictures.AddRange(info.ProfilePictures
            .Where(x => !ProfilePictures.Contains(x)));
        Birthday ??= info.Birthday;
        Country ??= info.Country;
        City ??= info.City;
        Profession ??= info.Profession;
        CareerStart ??= info.CareerStart;
        StillActive ??= info.StillActive;
        Aliases.AddRange(info.Aliases);
        Aliases.AddRange(info.Aliases
            .Where(x => !Aliases.Contains(x)));
        Aliases.Sort();
        Ethnicity ??= info.Ethnicity;
        HairColor ??= info.HairColor;
        EyeColor ??= info.EyeColor;
        MeasurementDetails ??= info.MeasurementDetails;
        Height ??= info.Height;
        Weight ??= info.Weight;
        Piercings ??= info.Piercings;
    }

}