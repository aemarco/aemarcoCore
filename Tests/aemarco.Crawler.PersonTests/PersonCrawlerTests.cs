using aemarco.Crawler.Common;

namespace aemarco.Crawler.PersonTests;

internal class PersonCrawlerTests : TestBase
{

    [Test]
    public void GetAvailableCrawlers_DeliversCorrectly()
    {
        var expected = typeof(IPersonCrawler).Assembly
            .GetTypes()
            .Where(x =>
                x.IsAssignableTo(typeof(IPersonCrawler)) &&
                x is { IsAbstract: false, IsClass: true } &&
                x.GetCustomAttribute<CrawlerAttribute>() != null)
            .Select(CrawlerInfo.FromCrawlerType)
            .OrderBy(x => x.Priority)
            .Select(x => x.FriendlyName)
            .ToList();
        var crawler = GetCrawler();

        var result = crawler.AvailableCrawlers
            .ToList();

        result.Should().Equal(expected);

        PrintJson(result);
    }


    [Test]
    public async Task StartAsync_MergesResults()
    {
        var crawler = GetCrawler();
        var result = await crawler.StartAsync("Foxi", "Di")
                     ?? throw new Exception("Did not get a PersonInfo");

        PrintJson(result);

        result.FirstName.Should().Be("Foxi");
        result.LastName.Should().Be("Di");
        result.Rating.Should().BeInRange(0, 10);
        result.Gender.Should().Be(Gender.Female);
        result.ProfilePictures.Should().BeEquivalentTo(new List<ProfilePicture>
        {

            new("https://thelordofporn.com/wp-content/uploads/2016/12/Foxi-Di-2.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di2.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di3.jpg"),
            //new("https://www.babepedia.com/pics/Foxy%20Di4.jpg"),
            new("https://m99.nudevista.com/_/866/158866_370.jpg"),
            new("https://i.analdin.com/contents/models/6590/s2_Foxi%20Di%201.jpg"),
            new("https://www.babesandstars.com/models/18000/18713/250x330.jpg")
        });
        result.Birthday.Should().Be(new DateOnly(1994, 9, 14));
        result.Country.Should().Be("Russia");
        result.City.Should().Be("St. Petersburg");
        result.Profession.Should().Be("Adult Model (Former), Porn Star (Former)");
        result.CareerStart.Should().Be(new DateOnly(2013, 1, 1));
        result.StillActive.Should().Be(false);
        result.Aliases.Should().BeEquivalentTo(new List<string>
        {
            "Angel", "Angel C", "Angel C. Metart", "Ekaterina D", "Ekaterina Ivanova", "Foxy B", "Foxy Di", "Foxy Dolce", "Inga", "Inna", "Kat", "Kate", "Katoa", "Katya Ivanova", "Kleine Punci", "Marisha", "Medina U", "Medina U Femjoy", "Medina U. Femjoy", "Nensi", "Nensi B", "Nensi B Medina", "Nensi B Met Art", "Nensi B. Medina", "Nensi B. Met Art"
        });
        result.Ethnicity.Should().Be("Caucasian");
        result.HairColor.Should().Be("Brown");
        result.EyeColor.Should().Be("Green");
        result.MeasurementDetails.ToString().Should().Be("86B-60-86");
        result.Height.Should().Be(157);
        result.Weight.Should().Be(45);
        result.Piercings.Should().Be("Navel");
        result.SocialLinks.Should().BeEquivalentTo(new[]
        {
            new SocialLink(SocialLinkKind.Twitter, "https://twitter.com/foxi_di"),
            new SocialLink(SocialLinkKind.Instagram, "https://instagram.com/foxy__di")
        });

        string.Join(",", result.CrawlerInfos.Select(x => x.FriendlyName))
            .Should().Be("TheLordOfPorn,Babepedia,Nudevista,Analdin,BabesAndStars");
        result.Errors.Should().BeEmpty();
    }


    private static PersonCrawler GetCrawler() => new();

}