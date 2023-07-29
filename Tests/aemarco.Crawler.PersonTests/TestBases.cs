namespace aemarco.Crawler.PersonTests;

internal abstract class PersonCrawlerTestsBase<T> : TestBase
{

    private readonly CrawlerInfo _crawlerInfo;
    private readonly string _nameToCrawl;
    protected PersonCrawlerTestsBase(
        string nameToCrawl)
    {
        _crawlerInfo = CrawlerInfo.FromCrawlerType(typeof(T));
        _nameToCrawl = nameToCrawl;
        ExpectedFirstName = _nameToCrawl.Split(' ')[0];
        ExpectedLastName = _nameToCrawl.Split(' ')[1];
    }


    [OneTimeSetUp]
    public async Task Init()
    {
        if (!_crawlerInfo.IsAvailable)
            return;


        var crawler = new PersonCrawler();
        crawler.AddPersonSiteFilter(_crawlerInfo.FriendlyName);
        Entry = await crawler.StartAsync(_nameToCrawl);
    }

    private PersonInfo? Entry { get; set; }



    [Test]
    public void Entry_Matches_PersonSite()
    {
        if (Entry is null)
            return;

        Entry.CrawlerInfos.FirstOrDefault().Should().Be(_crawlerInfo);
        PrintJson(Entry.CrawlerInfos);
    }

    [Test]
    public void Entry_HasNoErrors()
    {
        if (Entry is null)
            return;

        Entry.Errors.Should().BeEmpty();
        PrintJson(Entry.Errors);
    }

    private string ExpectedFirstName { get; init; }
    [Test]
    public void Crawler_Finds_FirstName()
    {
        if (Entry is null)
            return;

        Entry.FirstName.Should().Be(ExpectedFirstName);
        PrintJson(Entry.FirstName);
    }

    private string ExpectedLastName { get; init; }
    [Test]
    public void Crawler_Finds_LastName()
    {
        if (Entry is null)
            return;

        Entry.LastName.Should().Be(ExpectedLastName);
        PrintJson(Entry.LastName);
    }

    protected bool? ExpectedRating { get; init; }
    [Test]
    public void Crawler_Finds_Rating()
    {
        if (Entry is null)
            return;

        if (ExpectedRating is null)
        {
            NothingExpected(Entry.Rating);
            return;
        }

        if (ExpectedRating.Value)
            Entry.Rating.Should().BeInRange(0, 10);
        else
            Entry.Rating.Should().BeNull();
        PrintJson(Entry.Rating?.ToString());
    }

    protected Gender? ExpectedGender { get; init; }
    [Test]
    public void Crawler_Finds_Gender()
    {
        if (Entry is null)
            return;

        if (ExpectedGender is null)
        {
            NothingExpected(Entry.Gender);
            return;
        }

        Entry.Gender.Should().Be(ExpectedGender);
        PrintJson(Entry.Gender?.ToString());
    }

    protected DateOnly? ExpectedBirthday { get; init; }
    [Test]
    public void Crawler_Finds_Birthday()
    {
        if (Entry is null)
            return;

        if (!ExpectedBirthday.HasValue)
        {
            NothingExpected(Entry.Birthday);
            return;
        }

        Entry.Birthday.Should().Be(ExpectedBirthday);
        PrintJson(Entry.Birthday);
    }

    protected DateOnly? ExpectedCareerStart { get; init; }
    [Test]
    public void Crawler_Finds_CareerStart()
    {
        if (Entry is null)
            return;

        if (!ExpectedCareerStart.HasValue)
        {
            NothingExpected(Entry.CareerStart);
            return;
        }

        Entry.CareerStart.Should().Be(ExpectedCareerStart);
        PrintJson(Entry.CareerStart);
    }

    protected string? ExpectedCountry { get; init; }
    [Test]
    public void Crawler_Finds_Country()
    {
        if (Entry is null)
            return;

        if (ExpectedCountry is null)
        {
            NothingExpected(Entry.Country);
            return;
        }

        Entry.Country.Should().Be(ExpectedCountry);
        PrintJson(Entry.Country);
    }

    protected string? ExpectedCity { get; init; }
    [Test]
    public void Crawler_Finds_City()
    {
        if (Entry is null)
            return;

        if (ExpectedCity is null)
        {
            NothingExpected(Entry.City);
            return;
        }

        Entry.City.Should().Be(ExpectedCity);
        PrintJson(Entry.City);
    }

    protected string? ExpectedProfession { get; init; }
    [Test]
    public void Crawler_Finds_Profession()
    {
        if (Entry is null)
            return;

        if (ExpectedProfession is null)
        {
            NothingExpected(Entry.Profession);
            return;
        }

        Entry.Profession.Should().Be(ExpectedProfession);
        PrintJson(Entry.Profession);
    }

    protected string? ExpectedEthnicity { get; init; }
    [Test]
    public void Crawler_Finds_Ethnicity()
    {
        if (Entry is null)
            return;

        if (ExpectedEthnicity is null)
        {
            NothingExpected(Entry.Ethnicity);
            return;
        }

        Entry.Ethnicity.Should().Be(ExpectedEthnicity);
        PrintJson(Entry.Ethnicity);
    }

    protected string? ExpectedHairColor { get; init; }
    [Test]
    public void Crawler_Finds_Hair()
    {
        if (Entry is null)
            return;

        if (ExpectedHairColor is null)
        {
            NothingExpected(Entry.HairColor);
            return;
        }

        Entry.HairColor.Should().Be(ExpectedHairColor);
        PrintJson(Entry.HairColor);
    }

    protected string? ExpectedEyeColor { get; init; }
    [Test]
    public void Crawler_Finds_Eyes()
    {
        if (Entry is null)
            return;

        if (ExpectedEyeColor is null)
        {
            NothingExpected(Entry.EyeColor);
            return;
        }

        Entry.EyeColor.Should().Be(ExpectedEyeColor);
        PrintJson(Entry.EyeColor);
    }

    protected int? ExpectedHeight { get; init; }
    [Test]
    public void Crawler_Finds_Height()
    {
        if (Entry is null)
            return;

        if (!ExpectedHeight.HasValue)
        {
            NothingExpected(Entry.Height);
            return;
        }

        Entry.Height.Should().Be(ExpectedHeight);
        PrintJson(Entry.Height);
    }

    protected int? ExpectedWeight { get; init; }
    [Test]
    public void Crawler_Finds_Weight()
    {
        if (Entry is null)
            return;

        if (!ExpectedWeight.HasValue)
        {
            NothingExpected(Entry.Weight);
            return;
        }

        Entry.Weight.Should().Be(ExpectedWeight);
        PrintJson(Entry.Weight);
    }

    protected string? ExpectedMeasurementDetails { get; init; } = "";
    [Test]
    public void Crawler_Finds_Measurements()
    {
        if (Entry is null)
            return;

        if (ExpectedMeasurementDetails is null)
        {
            NothingExpected(Entry.MeasurementDetails);
            return;
        }

        Entry.MeasurementDetails.ToString().Should().Be(ExpectedMeasurementDetails);
        PrintJson(Entry.MeasurementDetails);
    }

    protected string? ExpectedPiercings { get; init; }
    [Test]
    public void Crawler_Finds_Piercings()
    {
        if (Entry is null)
            return;

        if (ExpectedPiercings is null)
        {
            NothingExpected(Entry.Piercings);
            return;
        }

        Entry.Piercings.Should().Be(ExpectedPiercings);
        PrintJson(Entry.Piercings);
    }

    protected bool? ExpectedStillActive { get; init; }
    [Test]
    public void Crawler_Finds_StillActive()
    {
        if (Entry is null)
            return;

        if (ExpectedStillActive is null)
        {
            NothingExpected(Entry.StillActive);
            return;
        }

        Entry.StillActive.Should().Be(ExpectedStillActive);
        PrintJson(Entry.StillActive);
    }

    protected List<string> ExpectedProfilePictures { get; set; } = new();
    [Test]
    public void Crawler_Finds_Pictures()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictures.Count == 0)
        {
            NothingExpected(Entry.ProfilePictures.Count == 0 ? null : Entry.ProfilePictures);
            return;
        }

        Entry.ProfilePictures.Select(x => x.Url).Should().BeEquivalentTo(ExpectedProfilePictures);
        PrintJson(Entry.ProfilePictures);
    }

    protected List<string> ExpectedAliases { get; set; } = new();
    [Test]
    public void Crawler_Finds_Aliases()
    {
        if (Entry is null)
            return;

        if (ExpectedAliases.Count == 0)
        {
            NothingExpected(Entry.Aliases.Count == 0 ? null : Entry.Aliases);
            return;
        }

        Entry.Aliases.Should().BeEquivalentTo(ExpectedAliases);
        PrintJson(Entry.Aliases);
    }

    protected List<SocialLink> ExpectedSocialLinks { get; set; } = new();
    [Test]
    public void Crawler_Finds_SocialLinks()
    {
        if (Entry is null)
            return;

        if (ExpectedSocialLinks.Count == 0)
        {
            NothingExpected(Entry.SocialLinks.Count == 0 ? null : Entry.SocialLinks);
            return;
        }

        Entry.SocialLinks.Should().BeEquivalentTo(ExpectedSocialLinks);
        PrintJson(Entry.SocialLinks);
    }

}

internal abstract class TestBase
{
    protected static void NothingExpected(object? found)
    {
        if (found is null)
            return;

        Assert.Warn($"""
                Expected Nothing but found: {GetTypeName(found)}
                {JsonConvert.SerializeObject(found, Formatting.Indented)}
                """);
    }

    protected static void PrintJson(object? obj)
    {
        if (obj is null)
        {
            TestContext.Out.WriteLine("Passed with: null");
            return;
        }

        TestContext.Out.WriteLine($"""
            Passed with: {GetTypeName(obj)}
             {JsonConvert.SerializeObject(obj, Formatting.Indented)}
            """);
    }


    private static string GetTypeName(object obj)
    {
        var type = obj.GetType();
        if (obj is not IEnumerable or string)
            return type.Name;

        var elementType = GetCollectionElementType(type);
        return $"{elementType?.Name ?? "UnknownType"}[]";
    }
    private static Type? GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType();


        // Handle collections implementing IEnumerable<T>
        var genericArguments = collectionType.GetGenericArguments();
        if (genericArguments.Length > 0)
            return genericArguments[0];


        // Handle non-generic collections implementing IEnumerable
        if (typeof(IEnumerable).IsAssignableFrom(collectionType))
        {
            foreach (var interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
        }
        return null;
    }


}