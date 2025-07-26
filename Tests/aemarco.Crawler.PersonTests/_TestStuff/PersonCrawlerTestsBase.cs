namespace aemarco.Crawler.PersonTests._TestStuff;

internal abstract class PersonCrawlerTestsBase<T> : TestBase
{

    private readonly string _firstName;
    private readonly string _lastName;
    private readonly CrawlerInfo _crawlerInfo;
    protected PersonCrawlerTestsBase(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        _crawlerInfo = CrawlerInfo.FromCrawlerType(typeof(T));
    }


    [OneTimeSetUp]
    public async Task Init()
    {
        var crawler = new PersonCrawler();
        crawler.AddPersonSiteFilter(_crawlerInfo.FriendlyName);
        Entry = await crawler.StartAsync(_firstName, _lastName);
    }

    private PersonInfo? Entry { get; set; }


    [Test]
    public void Entry_Matches_PersonSite()
    {
        if (Entry is null)
            return;

        Entry.CrawlerInfos.FirstOrDefault().Should().Be(_crawlerInfo);
        PrintPassed(Entry.CrawlerInfos);
    }

    [Test]
    public void Entry_HasNoErrors()
    {
        if (Entry is null)
            return;

        Entry.Errors.Should().BeEmpty();
        PrintPassed(Entry.Errors);
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
        PrintPassed(Entry.Rating?.ToString());
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
        PrintPassed(Entry.Gender?.ToString());
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
        PrintPassed(Entry.Birthday);
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
        PrintPassed(Entry.CareerStart);
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
        PrintPassed(Entry.Country);
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
        PrintPassed(Entry.City);
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
        PrintPassed(Entry.Profession);
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
        PrintPassed(Entry.Ethnicity);
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
        PrintPassed(Entry.HairColor);
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
        PrintPassed(Entry.EyeColor);
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
        PrintPassed(Entry.Height);
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
        PrintPassed(Entry.Weight);
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
        PrintPassed(Entry.MeasurementDetails);
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
        PrintPassed(Entry.Piercings);
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
        PrintPassed(Entry.StillActive);
    }

    protected List<string> ExpectedProfilePictures { get; set; } = [];
    [Test]
    public void Crawler_Finds_Pictures()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictures.Count == 0)
        {
            NothingExpected(null);
            return;
        }
        Entry.ProfilePictures.Select(x => x.Url).Should().BeEquivalentTo(ExpectedProfilePictures);
        PrintPassed(Entry.ProfilePictures);
    }

    protected int? ExpectedProfilePictureMinCount { get; set; }
    [Test]
    public void Crawler_Finds_AtLeastMinCount()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictureMinCount is null)
        {
            NothingExpected(null);
            return;
        }

        Entry.ProfilePictures.Count.Should().BeGreaterOrEqualTo(ExpectedProfilePictureMinCount.Value);
        PrintPassed(Entry.ProfilePictures);
    }


    protected List<string> ExpectedAliases { get; set; } = [];
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
        PrintPassed(Entry.Aliases);
    }

    protected List<SocialLink> ExpectedSocialLinks { get; set; } = [];
    [Test]
    public void Crawler_Finds_SocialLinks()
    {
        //ExpectedSocialLinks = [];

        //Assume.That(ExpectedSocialLinks.Count > 0);




        //Assert.Ignore();
        //return;


        if (Entry is null)
            return;

        if (ExpectedSocialLinks.Count == 0)
        {
            NothingExpected(Entry.SocialLinks.Count == 0 ? null : Entry.SocialLinks);
            return;
        }

        Entry.SocialLinks.Should().BeEquivalentTo(ExpectedSocialLinks);
        PrintPassed(Entry.SocialLinks);
    }

}