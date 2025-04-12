namespace aemarco.Crawler.Person.Model;

public record PersonInfo
{
    //TODO Profession should be a list
    //TODO Piercings should be a list

    public string? FirstName { get; internal set; }
    public string? LastName { get; internal set; }


    /// <summary>
    /// rating in range 0 - 10 or null
    /// </summary>
    public double? Rating { get; internal set; }
    public Gender? Gender { get; internal set; }
    public List<ProfilePicture> ProfilePictures { get; } = [];
    public DateOnly? Birthday { get; internal set; }
    public string? Country { get; internal set; }
    public string? City { get; internal set; }
    public string? Profession { get; internal set; }
    public DateOnly? CareerStart { get; internal set; }
    public bool? StillActive { get; internal set; }
    public List<string> Aliases { get; } = [];
    public string? Ethnicity { get; internal set; }
    public string? HairColor { get; internal set; }
    public string? EyeColor { get; internal set; }
    public MeasurementDetails MeasurementDetails { get; internal set; } = MeasurementDetails.Empty;
    public int? Height { get; internal set; }
    public int? Weight { get; internal set; }
    public string? Piercings { get; internal set; }
    public List<SocialLink> SocialLinks { get; private set; } = [];


    internal void Merge(IEnumerable<PersonInfo> items)
    {
        foreach (var entry in items
                     .OrderBy(x => x.CrawlerInfos.First().Priority))
        {
            Merge(entry);
        }
    }
    private void Merge(PersonInfo info)
    {
        if (info.FirstName != FirstName || info.LastName != LastName)
        {//not a first degree match

            if (!Aliases.Contains($"{info.FirstName} {info.LastName}") &&
                !info.Aliases.Contains($"{FirstName} {LastName}"))
            {//not a second degree match

                //we ignore this info, as it´s not matching our entry
                return;
            }
        }

        CrawlerInfos.AddRange(info.CrawlerInfos.Where(x => !CrawlerInfos.Contains(x)));

        if (Rating is null && info.Rating is { } rating and >= 0 and <= 10)
            Rating = rating;
        Gender ??= info.Gender;
        ProfilePictures.AddRange(info.ProfilePictures.Where(x => !ProfilePictures.Contains(x)));
        Birthday ??= info.Birthday;
        Country ??= info.Country;
        City ??= info.City;
        Profession ??= info.Profession;
        CareerStart ??= info.CareerStart;
        StillActive ??= info.StillActive;
        Aliases.AddRange(info.Aliases.Where(x => !Aliases.Contains(x)));
        Aliases.RemoveAll(x => x == $"{FirstName} {LastName}");
        Aliases.Sort();
        Ethnicity ??= info.Ethnicity;
        HairColor ??= info.HairColor;
        EyeColor ??= info.EyeColor;
        MeasurementDetails = MeasurementDetails.Combine(info.MeasurementDetails);
        Height ??= info.Height;
        Weight ??= info.Weight;
        Piercings ??= info.Piercings;
        SocialLinks.AddRange(info.SocialLinks);
        SocialLinks = SocialLinks
            .GroupBy(x => x.Kind)
            .Select(x => x.OrderBy(s => s.Url.Length).First())
            .ToList();
        SocialLinks.Sort();
    }



    public List<CrawlerInfo> CrawlerInfos { get; } = [];
    public List<Exception> Errors { get; } = [];

}