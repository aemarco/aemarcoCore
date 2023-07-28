using aemarco.Crawler.Model;

namespace aemarco.Crawler.PersonTests;

internal class PersonCrawlerTests : TestBase
{

    [Test]
    public void GetAvailableCrawlers_DeliversCorrectly()
    {
        var expected = Assembly
            .GetAssembly(typeof(PersonCrawlerBase))!
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(PersonCrawlerBase)))
            .Select(CrawlerInfo.FromCrawlerType)
            .Where(x => x.IsAvailable)
            .OrderBy(x => x.Priority)
            .Select(x => x.FriendlyName)
            .ToList();

        var result = PersonCrawler.GetAvailableCrawlers().ToList();
        result.Should().Equal(expected);

        PrintJson(result);
    }


    [Test]
    public async Task StartAsync_MergesResults()
    {
        var crawler = new PersonCrawler();
        var result = await crawler.StartAsync("Foxi Di")
                     ?? throw new Exception("Did not get a PersonInfo");

        PrintJson(result);

        result.FirstName.Should().Be("Foxi");
        result.LastName.Should().Be("Di");
        result.Gender.Should().Be(Gender.Female);
        result.ProfilePictures.Should().BeEquivalentTo(new List<ProfilePicture>
        {
            //new("https://www.babepedia.com/pics/Foxy%20Di.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di2.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di3.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di4.jpg"),
            new("https://b99.nudevista.com/_/866/158866_370.jpg"),
            new("https://www.babesandstars.com/models/18000/18713/250x330.jpg")
        });
        result.Birthday.Should().Be(new DateOnly(1994, 9, 14));
        result.Country.Should().Be("Russia");
        result.City.Should().Be("Saint Petersburg");
        result.Profession.Should().Be("Adult Model");
        result.CareerStart.Should().Be(new DateOnly(2013, 1, 1));
        result.StillActive.Should().BeNull();
        result.Aliases.Should().BeEquivalentTo(new List<string>
        {
            "Angel C",
            "Angel C. Metart",
            "Foxi Di",
            "Foxy Di",
            "Foxy Dolce",
            "Inna",
            "Kleine Punci",
            "Medina U",
            "Medina U Femjoy",
            "Medina U. Femjoy",
            "Nensi B",
            "Nensi B Met Art",
            "Nensi B. Medina",
            "Nensi B. Met Art"
        });
        result.Ethnicity.Should().Be("Caucasian");
        result.HairColor.Should().Be("Brown");
        result.EyeColor.Should().Be("Blue");
        result.MeasurementDetails.ToString().Should().Be("86B-60-86");
        result.Height.Should().Be(165);
        result.Weight.Should().Be(44);
        result.Piercings.Should().Be("Navel");
        result.Errors.Should().BeEmpty();
    }




    [Ignore("needs some work")]
    [Test]
    public async Task StartAsync_MergesResults_WithBabepedia()
    {
        var crawler = new PersonCrawler();
        var result = await crawler.StartAsync("Foxi Di")
                     ?? throw new Exception("Did not get a PersonInfo");

        PrintJson(result);

        result.FirstName.Should().Be("Foxi");
        result.LastName.Should().Be("Di");
        result.Gender.Should().Be(Gender.Female);
        result.ProfilePictures.Should().BeEquivalentTo(new List<ProfilePicture>
        {
            new("https://www.babepedia.com/pics/Foxy%20Di.jpg"),
            new("https://www.babepedia.com/pics/Foxy%20Di2.jpg"),
            new("https://www.babepedia.com/pics/Foxy%20Di3.jpg"),
            new("https://www.babepedia.com/pics/Foxy%20Di4.jpg"),
            new("https://b99.nudevista.com/_/866/158866_370.jpg"),
            new("https://www.babesandstars.com/models/18000/18713/250x330.jpg")
        });
        result.Birthday.ToString().Should().Be("14.09.1994");
        result.Country.Should().Be("Russian Federation");
        result.City.Should().Be("St. Petersburg");
        result.Profession.Should().Be("Adult Model, Glamour Model, Porn Star");
        result.CareerStart.ToString().Should().Be("01.01.2013");
        result.StillActive.Should().BeTrue();
        result.Aliases.Should().BeEquivalentTo(new List<string>
        {
            "Angel C",
            "Angel C. Metart",
            "Ekaterina D",
            "Foxi Di",
            "Foxy Di",
            "Foxy Dolce",
            "Inna",
            "Katoa",
            "Katya Ivanova",
            "Kleine Punci",
            "Medina U",
            "Medina U Femjoy",
            "Medina U. Femjoy",
            "Nensi B",
            "Nensi B Met Art",
            "Nensi B. Medina",
            "Nensi B. Met Art"
        });
        result.Ethnicity.Should().Be("Caucasian");
        result.HairColor.Should().Be("Brown");
        result.EyeColor.Should().Be("Hazel");
        result.MeasurementDetails.ToString().Should().Be("86B-60-86");
        result.Height.Should().Be(157);
        result.Weight.Should().Be(49);
        result.Piercings.Should().Be("Navel");
        result.Errors.Should().BeEmpty();
    }

}