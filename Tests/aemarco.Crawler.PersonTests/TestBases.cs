namespace aemarco.Crawler.PersonTests;




//[Ignore("Crawler obsolete")]
//[Obsolete]




internal abstract class PersonCrawlerTestsBase<T> : TestBase
{

    private readonly string _nameToCrawl;
    private readonly CrawlerInfo _crawlerInfo;
    protected PersonCrawlerTestsBase(string nameToCrawl)
    {
        _nameToCrawl = nameToCrawl;
        ExpectedFirstName = _nameToCrawl.Split(' ')[0];
        ExpectedLastName = _nameToCrawl.Split(' ')[1];
        _crawlerInfo = CrawlerInfo.FromCrawlerType(typeof(T));
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

        Entry.CrawlerInfos.FirstOrDefault().Should().Be(_crawlerInfo);
        PrintJson(Entry.CrawlerInfos);
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
    public void Crawler_Finds_Place()
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

    protected string? ExpectedMeasurementDetails { get; init; }
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
}

internal abstract class TestBase
{
    protected static void NothingExpected(object? found)
    {
        PrintJson(new
        {
            Expected = "Noting",
            Found = found ?? "Nothing"
        });
    }

    protected static void PrintJson(object? obj)
    {
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                obj,
                Formatting.Indented));
    }

}