namespace aemarco.Crawler.Person.Crawlers;


[Crawler("IStripper", 50)]
internal class Stripper : SiteCrawlerBase
{

    private readonly Uri _uri = new("https://www.istripper.com");
    public Stripper(
        ICountryService countryService,
        ILogger<Stripper> logger)
        : base(countryService, logger)
    {

    }


    protected override async Task<PersonName[]> HandlePersonNameEntries(CancellationToken token)
    {
        var uri = new PageUri(_uri).WithHref("/models?sort=rating&search=");
        var page = await uri.NavigateAsync(token: token);
        List<PersonName> result = [];

        //first page of performers
        // order: rating desc

        foreach (var node in page.FindNodes("//div[@class='inner']/h2[@class='b']"))
        {
            var text = node.GetText().TitleCase();
            var (fn, ln) = PersonParser.FindNameInText(text);
            if (fn is null || ln is null)
                continue;

            result.Add(new PersonName($"{fn} {ln}"));
        }


        return [.. result.Distinct()];
    }

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/de/models/{name}"
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://www.istripper.com/de/models/Aletta-Ocean
        return result;
    }

    protected override Task HandlePersonEntry(PageDocument girlPage, CancellationToken token)
    {

        //Name
        var nameNode = girlPage.FindNode("//div[@class='trigger']/div/h1");
        UpdateName(nameNode);


        //Pic
        var picUri = girlPage
            .FindNode("//div[@class='container']/img")?
            .GetSrc();
        UpdateProfilePictures(picUri, 35, 39);

        //Rating
        if (girlPage.FindNode("//span[@class='star rate']")?.GetText() is { } ratingText &&
            PersonParser.FindRatingInText(ratingText) is { } rating)
            Result.Rating = rating * 2;

        //Data
        var dataNodes = girlPage.FindNodes("//ul[@class='info2']/li");
        Result.Gender = Gender.Female; //always female on this site
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();
            var text = node.GetText();

            if (text.Contains("Land:"))
                UpdateCountry(text.Except("Land:").TitleCase());
            else if (text.Contains("Stadt:"))
                Result.City = text.Except("Stadt:").TitleCase();
            else if (text.Contains("Größe"))
                Result.Height = PersonParser.FindHeightInText(text.Except("Größe").TitleCase());
            else if (text.Contains("Gewicht:"))
                Result.Weight = PersonParser.FindWeightInText(text.Except("Gewicht:").TitleCase());
            else if (text.Contains("Maße:"))
                UpdateMeasurements(text.Except("Maße:").TitleCase());
        }
        return Task.CompletedTask;

    }

}