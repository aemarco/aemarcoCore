namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Pornstarbyface", 10)]
internal class Pornstarbyface : SiteCrawlerBase
{

    private readonly Uri _uri = new("https://pornstarbyface.com");
    public Pornstarbyface(
        ICountryService countryService,
        ILogger<Pornstarbyface> logger)
        : base(countryService, logger)
    {

    }


    protected override PageUri GetGirlUri(string name)
    {
        name = name
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref($"/girls/{name}");

        //https://pornstarbyface.com/girls/Ariel-Rebel
        return result;
    }

    protected override Task HandlePersonEntry(PageDocument girlPage, CancellationToken token)
    {
        var starInfo = girlPage.FindNode("//div[@class='star-info']");
        if (starInfo is null)
            return Task.CompletedTask;

        //Name
        var nameNode = starInfo.FindNode("./h5");
        UpdateName(nameNode);

        //Pic
        UpdateProfilePictures(girlPage
            .FindNode("//div[@class='col-lg-3 profile-image']/img[@class='img-responsive']")?
            .GetSrc());

        //Data
        var dataNodes = starInfo.FindNodes("./div/b");
        Result.Gender = Gender.Female; //always female on this site
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            var label = node.GetText();
            if (string.IsNullOrWhiteSpace(label))
                continue;

            if (node.Parent().NextSibling() is not { } content)
                continue;

            var text = content.GetText().TitleCase();
            Action act = label switch
            {
                "Aliases" => () => Result.Aliases.AddRange(text.SplitList()),
                "Country" => () => UpdateCountry(text),
                "State" => () => Result.City = text,
                "Birthday" => () => Result.Birthday = text.ToDateOnly(),
                "Ethnicity" => () => Result.Ethnicity = text,
                "Eye" => () => Result.EyeColor = text,
                "Hair" => () => Result.HairColor = text,
                "Height" => () => Result.Height = PersonParser.FindHeightInText(text),
                "Weight" => () => Result.Weight = PersonParser.FindWeightInText(text),
                "Measurements" => () => UpdateMeasurements(text, true),
                "Cup" => () => UpdateMeasurements(text),
                _ => () => { }
            };
            act();
        }

        return Task.CompletedTask;
    }

}