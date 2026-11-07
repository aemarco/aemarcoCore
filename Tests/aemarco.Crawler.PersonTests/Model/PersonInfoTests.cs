namespace aemarco.Crawler.PersonTests.Model;

internal class PersonInfoTests
{

    [Test]
    public void Merge_CorrectOrder()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Count.ShouldBe(2);
        sut.CrawlerInfos.Select(x => x.Priority).ShouldBeInOrder();

        sut.CrawlerInfos.Dump();
    }
    [Test]
    public void Merge_CorrectInfos()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Count.ShouldBe(2);
        sut.CrawlerInfos.Select(x => x.FriendlyName).ShouldBe(["First", "Middle"]);

        sut.CrawlerInfos.Dump();
    }

    [Test]
    public void Merge_KeepsName()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.FirstName.ShouldBe("Foxi");
        sut.LastName.ShouldBe("Di");

        sut.Dump();
    }

    [Test]
    public void Merge_SecondDegreeMatch()
    {
        var sut = GetSut();
        var infos = GetInfos();
        infos[0].Aliases.Add("Foxy Di");

        sut.Merge(infos);

        sut.CrawlerInfos.Count.ShouldBe(3);
        //because last "Foxy Di" is an alias match on earlier info
        sut.CrawlerInfos.Select(x => x.FriendlyName).ShouldContain("Last");
        sut.CrawlerInfos.Dump();
    }

    [Test]
    public void Merge_RemovesNotMatching()
    {
        var sut = GetSut();
        var infos = GetInfos();

        sut.Merge(infos);

        sut.CrawlerInfos.Count.ShouldBe(2);
        sut.CrawlerInfos.Select(x => x.FriendlyName).ShouldNotContain("Last");

        sut.CrawlerInfos.Dump();
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
        sut.Rating.ShouldBe(expected);
        sut.Rating.Dump();
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

        sut.Gender.ShouldBe(Gender.Female);
        sut.ProfilePictures
            .Select(x => x.Url)
            .ShouldBe(["https://example.com/1.jpg", "https://example.com/2.jpg"]);
        sut.Birthday.ShouldBe(new DateOnly(1990, 1, 1));
        sut.Country.ShouldBe("Germany");
        sut.City.ShouldBe("Berlin");
        sut.Profession.ShouldBe("DoStuff");
        sut.CareerStart.ShouldBe(new DateOnly(2010, 1, 1));
        sut.StillActive.ShouldBe(false);
        sut.Aliases.ShouldBe(["1", "2"]);
        sut.Ethnicity.ShouldBe("Marsian");
        sut.HairColor.ShouldBe("Golden");
        sut.EyeColor.ShouldBe("Purple");
        sut.MeasurementDetails.ToString().ShouldBe("1B-2-3");
        sut.Height.ShouldBe(160);
        sut.Weight.ShouldBe(40);
        sut.Piercings.ShouldBe("Lip");
        sut.SocialLinks
            .Select(x => x.Url)
            .ShouldBe(["https://example.com/unknown", "https://example.com/official"]);

        sut.Dump();
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
