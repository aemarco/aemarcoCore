using aemarco.TestBasics;

namespace aemarco.Crawler.PersonTests.Model;

internal class PersonInfoTests
{

    [Test]
    public void Merge_CorrectOrder()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Should().HaveCount(2);
        sut.CrawlerInfos.Select(x => x.Priority).Should().BeInAscendingOrder();

        TestHelper.PrintPassed(sut.CrawlerInfos);
    }
    [Test]
    public void Merge_CorrectInfos()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Should().HaveCount(2);
        sut.CrawlerInfos.Select(x => x.FriendlyName).Should().Equal("First", "Middle");

        TestHelper.PrintPassed(sut.CrawlerInfos);
    }

    [Test]
    public void Merge_KeepsName()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.FirstName.Should().Be("Foxi");
        sut.LastName.Should().Be("Di");

        TestHelper.PrintPassed(sut);
    }

    [Test]
    public void Merge_SecondDegreeMatch()
    {
        var sut = GetSut();
        var infos = GetInfos();
        infos[0].Aliases.Add("Foxy Di");

        sut.Merge(infos);

        sut.CrawlerInfos.Should().HaveCount(3);
        //because last "Foxy Di" is an alias match on earlier info
        sut.CrawlerInfos.Select(x => x.FriendlyName).Should().Contain("Last");
        TestHelper.PrintPassed(sut.CrawlerInfos);
    }

    [Test]
    public void Merge_RemovesNotMatching()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Should().HaveCount(2);
        sut.CrawlerInfos.Select(x => x.FriendlyName).Should().NotContain("Last");

        TestHelper.PrintPassed(sut.CrawlerInfos);
    }


    //null
    [TestCase(null, null, null)]
    //place
    [TestCase(7, null, 7)]
    [TestCase(null, 7, 7)]
    //order
    [TestCase(7, 8, 7)]
    [TestCase(null, 8, 8)]
    //outOfRange
    [TestCase(null, 10.1, null)]
    [TestCase(null, -0.1, null)]
    public void Merge_Rating(double? first, double? second, double? expected)
    {
        var sut = GetSut();
        var infos = GetInfos();
        infos[1].Rating = first;
        infos[0].Rating = second;


        sut.Merge(infos);
        sut.Rating.Should().Be(expected);
        TestHelper.PrintPassed(sut.Rating);
    }


    [TestCase(0)]
    [TestCase(1)]
    public void Merge_Infos(int infoIndex)
    {
        var sut = GetSut();
        var infos = GetInfos();
        infos[infoIndex].Gender = Gender.Female;
        infos[infoIndex].ProfilePictures.Add(new ProfilePicture("https://example.com/1.jpg"));
        infos[infoIndex].ProfilePictures.Add(new ProfilePicture("https://example.com/2.jpg"));
        infos[infoIndex].Birthday = new DateOnly(1990, 1, 1);
        infos[infoIndex].Country = "Germany";
        infos[infoIndex].City = "Berlin";
        infos[infoIndex].Profession = "DoStuff";
        infos[infoIndex].CareerStart = new DateOnly(2010, 1, 1);
        infos[infoIndex].StillActive = false;
        infos[infoIndex].Aliases.Add("1");
        infos[infoIndex].Aliases.Add("2");
        infos[infoIndex].Ethnicity = "Marsian";
        infos[infoIndex].HairColor = "Golden";
        infos[infoIndex].EyeColor = "Purple";
        infos[infoIndex].MeasurementDetails = MeasurementDetails.Parse("1B-2-3");
        infos[infoIndex].Height = 160;
        infos[infoIndex].Weight = 40;
        infos[infoIndex].Piercings = "Lip";
        infos[infoIndex].SocialLinks.Add(
            new SocialLink(SocialLinkKind.Unknown, "https://example.com/unknown"));
        infos[infoIndex].SocialLinks.Add(
            new SocialLink(SocialLinkKind.Official, "https://example.com/official"));



        sut.Merge(infos);

        sut.Gender.Should().Be(Gender.Female);
        sut.ProfilePictures
            .Select(x => x.Url)
            .Should().Equal("https://example.com/1.jpg", "https://example.com/2.jpg");
        sut.Birthday.Should().Be(new DateOnly(1990, 1, 1));
        sut.Country.Should().Be("Germany");
        sut.City.Should().Be("Berlin");
        sut.Profession.Should().Be("DoStuff");
        sut.CareerStart.Should().Be(new DateOnly(2010, 1, 1));
        sut.StillActive.Should().BeFalse();
        sut.Aliases.Should().Equal("1", "2");
        sut.Ethnicity.Should().Be("Marsian");
        sut.HairColor.Should().Be("Golden");
        sut.EyeColor.Should().Be("Purple");
        sut.MeasurementDetails.ToString().Should().Be("1B-2-3");
        sut.Height.Should().Be(160);
        sut.Weight.Should().Be(40);
        sut.Piercings.Should().Be("Lip");
        sut.SocialLinks
            .Select(x => x.Url)
            .Should().Equal("https://example.com/unknown", "https://example.com/official");

        TestHelper.PrintPassed(sut);
    }

    private static PersonInfo GetSut()
    {
        var result = new PersonInfo
        {
            FirstName = "Foxi",
            LastName = "Di"
        };
        return result;
    }
    private static PersonInfo[] GetInfos()
    {
        PersonInfo[] result =
        [
            new()
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Middle", 7) }
            },
            new()
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("First", 5) }
            },
            new()
            {
                FirstName = "Foxy",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Last", 99) }
            },
            new()
            {
                FirstName = "Not",
                LastName = "Matching",
                CrawlerInfos = { new CrawlerInfo("Nope", 99) }
            }
        ];
        return result;
    }

}
