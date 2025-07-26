using aemarco.TestBasics;

namespace aemarco.Crawler.PersonTests._TestStuff;

internal abstract class PersonCrawlerTestsBase<T>
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
        Entry = await crawler.CrawlPerson(_firstName, _lastName);
    }

    private PersonInfo? Entry { get; set; }


    [Test]
    public void Entry_Matches_PersonSite()
    {
        if (Entry is null)
            return;

        Entry.CrawlerInfos.FirstOrDefault().Should().Be(_crawlerInfo);
        TestHelper.PrintPassed(Entry.CrawlerInfos);
    }

    [Test]
    public void Entry_HasNoErrors()
    {
        if (Entry is null)
            return;

        Entry.Errors.Should().BeEmpty();
        TestHelper.PrintPassed(Entry.Errors);
    }

    protected bool? ExpectedRating { get; init; }
    [Test]
    public void Crawler_Rating()
    {
        if (Entry is null)
            return;

        if (ExpectedRating is null)
        {
            TestHelper.NothingExpected(Entry.Rating);
            return;
        }

        if (ExpectedRating.Value)
            Entry.Rating.Should().BeInRange(0, 10);
        else
            Entry.Rating.Should().BeNull();
        TestHelper.PrintPassed(Entry.Rating?.ToString());
    }

    protected Gender? ExpectedGender { get; init; }
    [Test]
    public void Crawler_Gender()
    {
        if (Entry is null)
            return;

        if (ExpectedGender is null)
        {
            TestHelper.NothingExpected(Entry.Gender);
            return;
        }

        Entry.Gender.Should().Be(ExpectedGender);
        TestHelper.PrintPassed(Entry.Gender?.ToString());
    }

    protected DateOnly? ExpectedBirthday { get; init; }
    [Test]
    public void Crawler_Birthday()
    {
        if (Entry is null)
            return;

        if (!ExpectedBirthday.HasValue)
        {
            TestHelper.NothingExpected(Entry.Birthday);
            return;
        }

        Entry.Birthday.Should().Be(ExpectedBirthday);
        TestHelper.PrintPassed(Entry.Birthday);
    }

    protected DateOnly? ExpectedCareerStart { get; init; }
    [Test]
    public void Crawler_CareerStart()
    {
        if (Entry is null)
            return;

        if (!ExpectedCareerStart.HasValue)
        {
            TestHelper.NothingExpected(Entry.CareerStart);
            return;
        }

        Entry.CareerStart.Should().Be(ExpectedCareerStart);
        TestHelper.PrintPassed(Entry.CareerStart);
    }

    protected string? ExpectedCountry { get; init; }
    [Test]
    public void Crawler_Country()
    {
        if (Entry is null)
            return;

        if (ExpectedCountry is null)
        {
            TestHelper.NothingExpected(Entry.Country);
            return;
        }

        Entry.Country.Should().Be(ExpectedCountry);
        TestHelper.PrintPassed(Entry.Country);
    }

    protected string? ExpectedCity { get; init; }
    [Test]
    public void Crawler_City()
    {
        if (Entry is null)
            return;

        if (ExpectedCity is null)
        {
            TestHelper.NothingExpected(Entry.City);
            return;
        }

        Entry.City.Should().Be(ExpectedCity);
        TestHelper.PrintPassed(Entry.City);
    }

    protected string? ExpectedProfession { get; init; }
    [Test]
    public void Crawler_Profession()
    {
        if (Entry is null)
            return;

        if (ExpectedProfession is null)
        {
            TestHelper.NothingExpected(Entry.Profession);
            return;
        }

        Entry.Profession.Should().Be(ExpectedProfession);
        TestHelper.PrintPassed(Entry.Profession);
    }

    protected string? ExpectedEthnicity { get; init; }
    [Test]
    public void Crawler_Ethnicity()
    {
        if (Entry is null)
            return;

        if (ExpectedEthnicity is null)
        {
            TestHelper.NothingExpected(Entry.Ethnicity);
            return;
        }

        Entry.Ethnicity.Should().Be(ExpectedEthnicity);
        TestHelper.PrintPassed(Entry.Ethnicity);
    }

    protected string? ExpectedHairColor { get; init; }
    [Test]
    public void Crawler_Hair()
    {
        if (Entry is null)
            return;

        if (ExpectedHairColor is null)
        {
            TestHelper.NothingExpected(Entry.HairColor);
            return;
        }

        Entry.HairColor.Should().Be(ExpectedHairColor);
        TestHelper.PrintPassed(Entry.HairColor);
    }

    protected string? ExpectedEyeColor { get; init; }
    [Test]
    public void Crawler_Eyes()
    {
        if (Entry is null)
            return;

        if (ExpectedEyeColor is null)
        {
            TestHelper.NothingExpected(Entry.EyeColor);
            return;
        }

        Entry.EyeColor.Should().Be(ExpectedEyeColor);
        TestHelper.PrintPassed(Entry.EyeColor);
    }

    protected int? ExpectedHeight { get; init; }
    [Test]
    public void Crawler_Height()
    {
        if (Entry is null)
            return;

        if (!ExpectedHeight.HasValue)
        {
            TestHelper.NothingExpected(Entry.Height);
            return;
        }

        Entry.Height.Should().Be(ExpectedHeight);
        TestHelper.PrintPassed(Entry.Height);
    }

    protected int? ExpectedWeight { get; init; }
    [Test]
    public void Crawler_Weight()
    {
        if (Entry is null)
            return;

        if (!ExpectedWeight.HasValue)
        {
            TestHelper.NothingExpected(Entry.Weight);
            return;
        }

        Entry.Weight.Should().Be(ExpectedWeight);
        TestHelper.PrintPassed(Entry.Weight);
    }

    protected string? ExpectedMeasurementDetails { get; init; } = "";
    [Test]
    public void Crawler_Measurements()
    {
        if (Entry is null)
            return;

        if (ExpectedMeasurementDetails is null)
        {
            TestHelper.NothingExpected(Entry.MeasurementDetails);
            return;
        }

        Entry.MeasurementDetails.ToString().Should().Be(ExpectedMeasurementDetails);
        TestHelper.PrintPassed(Entry.MeasurementDetails);
    }

    protected string? ExpectedPiercings { get; init; }
    [Test]
    public void Crawler_Piercings()
    {
        if (Entry is null)
            return;

        if (ExpectedPiercings is null)
        {
            TestHelper.NothingExpected(Entry.Piercings);
            return;
        }

        Entry.Piercings.Should().Be(ExpectedPiercings);
        TestHelper.PrintPassed(Entry.Piercings);
    }

    protected bool? ExpectedStillActive { get; init; }
    [Test]
    public void Crawler_StillActive()
    {
        if (Entry is null)
            return;

        if (ExpectedStillActive is null)
        {
            TestHelper.NothingExpected(Entry.StillActive);
            return;
        }

        Entry.StillActive.Should().Be(ExpectedStillActive);
        TestHelper.PrintPassed(Entry.StillActive);
    }

    protected List<string> ExpectedProfilePictures { get; init; } = [];
    [Test]
    public void Crawler_Pictures()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictures.Count == 0)
        {
            TestHelper.NothingExpected(null);
            return;
        }
        Entry.ProfilePictures.Select(x => x.Url).Should().BeEquivalentTo(ExpectedProfilePictures);
        TestHelper.PrintPassed(Entry.ProfilePictures);
    }

    protected int? ExpectedProfilePictureMinCount { get; init; }
    [Test]
    public void Crawler_AtLeastMinCount()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictureMinCount is null)
        {
            TestHelper.NothingExpected(null);
            return;
        }

        Entry.ProfilePictures.Count.Should().BeGreaterOrEqualTo(ExpectedProfilePictureMinCount.Value);
        TestHelper.PrintPassed(Entry.ProfilePictures);
    }


    protected List<string> ExpectedAliases { get; init; } = [];
    [Test]
    public void Crawler_Aliases()
    {
        if (Entry is null)
            return;

        if (ExpectedAliases.Count == 0)
        {
            TestHelper.NothingExpected(Entry.Aliases.Count == 0 ? null : Entry.Aliases);
            return;
        }

        Entry.Aliases.Should().BeEquivalentTo(ExpectedAliases);
        TestHelper.PrintPassed(Entry.Aliases);
    }

    protected List<SocialLink> ExpectedSocialLinks { get; init; } = [];
    [Test]
    public void Crawler_SocialLinks()
    {

        if (Entry is null)
            return;

        if (ExpectedSocialLinks.Count == 0)
        {
            TestHelper.NothingExpected(Entry.SocialLinks.Count == 0 ? null : Entry.SocialLinks);
            return;
        }

        Entry.SocialLinks.Should().BeSubsetOf(ExpectedSocialLinks);
        TestHelper.PrintPassed(Entry.SocialLinks);
    }


}