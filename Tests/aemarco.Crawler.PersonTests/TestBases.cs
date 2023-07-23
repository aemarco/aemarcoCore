namespace aemarco.Crawler.PersonTests;

internal abstract class PersonCrawlerTestsBase<T> : TestBase
{

    private readonly string _nameToCrawl;
    private readonly CrawlerInfo _crawlerInfo;
    protected PersonCrawlerTestsBase(string nameToCrawl)
    {

        _nameToCrawl = nameToCrawl;
        ExpectedFirstName = _nameToCrawl.Split(' ')[0];
        ExpectedLastName = _nameToCrawl.Split(' ')[1];

        var type = PersonCrawler
                       .GetAvailableCrawlerTypes()
                       .FirstOrDefault(x => x.FullName == typeof(T).FullName)
                   ?? throw new Exception($"Type {typeof(T).FullName} not available");
        _crawlerInfo = CrawlerInfo.FromCrawlerType(type)
            ?? throw new Exception("Could not get CrawlerInfo");
    }


    [OneTimeSetUp]
    public async Task Init()
    {
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

        Entry.CrawlerInfos.Count.Should().Be(1);



        Assert.AreEqual(_crawlerInfo, Entry.CrawlerInfos[0]);
        PrintJson(Entry.CrawlerInfos[0]);
    }

    protected string ExpectedFirstName { get; init; }
    [Test]
    public void Crawler_Finds_FirstName()
    {
        if (Entry is null)
            return;
        Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
        PrintJson(Entry.FirstName);
    }

    protected string ExpectedLastName { get; init; }
    [Test]
    public void Crawler_Finds_LastName()
    {
        if (Entry is null)
            return;
        Assert.AreEqual(ExpectedLastName, Entry.LastName);
        PrintJson(Entry.LastName);
    }

    protected Gender? ExpectedGender { get; set; }
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
        Assert.AreEqual(ExpectedGender, Entry.Gender);
        PrintJson(Entry.Gender?.ToString());
    }

    protected DateOnly? ExpectedBirthday { get; set; }
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

        Assert.AreEqual(ExpectedBirthday!.Value, Entry.Birthday);
        PrintJson(Entry.Birthday);
    }

    protected DateOnly? ExpectedCareerStart { get; set; }
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

        Assert.AreEqual(ExpectedCareerStart.Value, Entry.CareerStart);
        PrintJson(Entry.CareerStart);

    }

    protected string? ExpectedCountry { get; set; }
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
        Assert.AreEqual(ExpectedCountry, Entry.Country);
        PrintJson(Entry.Country);


    }

    protected string? ExpectedCity { get; set; }
    [Test]
    public void Crawler_Finds_Place()
    {
        if (Entry is null)
            return;
        if (ExpectedCity is null)
        {
            NothingExpected(Entry.City);
            return;
        }
        Assert.AreEqual(ExpectedCity, Entry.City);
        PrintJson(Entry.City);
    }

    protected string? ExpectedProfession { get; set; }
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
        Assert.AreEqual(ExpectedProfession, Entry.Profession);
        PrintJson(Entry.Profession);
    }

    protected string? ExpectedEthnicity { get; set; }
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
        Assert.AreEqual(ExpectedEthnicity, Entry.Ethnicity);
        PrintJson(Entry.Ethnicity);
    }

    protected string? ExpectedHairColor { get; set; }
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
        Assert.AreEqual(ExpectedHairColor, Entry.HairColor);

        PrintJson(Entry.HairColor);

    }


    protected string? ExpectedEyeColor { get; set; }
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
        Assert.AreEqual(ExpectedEyeColor, Entry.EyeColor);
        PrintJson(Entry.EyeColor);

    }

    protected int? ExpectedHeight { get; set; }
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
        Assert.AreEqual(ExpectedHeight!.Value, Entry.Height);

        PrintJson(Entry.Height);

    }

    protected int? ExpectedWeight { get; set; }
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
        Assert.AreEqual(ExpectedWeight!.Value, Entry.Weight);
        PrintJson(Entry.Weight);

    }

    protected string? ExpectedMeasurementDetails { get; set; }
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
        Assert.AreEqual(ExpectedMeasurementDetails, Entry.MeasurementDetails.ToString());
        PrintJson(Entry.MeasurementDetails);
    }


    protected string? ExpectedPiercings { get; set; }
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
        Assert.AreEqual(ExpectedPiercings, Entry.Piercings);

        PrintJson(Entry.Piercings);

    }

    protected bool? ExpectedStillActive { get; set; }
    [Test]
    public void Crawler_Finds_StillActive()
    {
        if (Entry is null)
            return;

        Assert.AreEqual(ExpectedStillActive, Entry.StillActive);
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

        foreach (var url in ExpectedProfilePictures)
        {
            Entry.ProfilePictures.Should().Contain(x => x.Url == url);
        }
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

        foreach (var al in ExpectedAliases)
        {
            Entry.Aliases.Should().Contain(al);
        }
        PrintJson(Entry.Aliases);

    }


    private static void NothingExpected(object? found)
    {
        PrintJson(new
        {
            Expected = "Noting",
            Found = found ?? "Nothing"
        });
    }
}

internal abstract class TestBase
{
    protected static void PrintJson(object? obj)
    {
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                obj,
                Formatting.Indented));
    }
}