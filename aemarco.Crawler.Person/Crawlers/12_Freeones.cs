namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Freeones", 12)]
internal class Freeones : SiteCrawlerBase
{

    private readonly Uri _uri = new("https://www.freeones.com");
    public Freeones(
        ICountryService countryService,
        ILogger<Freeones> logger)
        : base(countryService, logger)
    {

    }


    protected override async Task<PersonName[]> HandlePersonNameEntries(CancellationToken token)
    {
        var uri = new PageUri(_uri).WithHref("/de/performers??s=rank.currentRank&l=96&q=&f[performerType]=babe&r[age]=18,25&filter_mode[performerType]=and&filter_mode[global]=and");
        var page = await uri.NavigateAsync(token: token);
        List<PersonName> result = [];

        //first page of performers
        // order: currentRank desc
        // amount = 96
        // filter performerType = babe (no trans, no male)
        // filter age = 18-25

        foreach (var node in page.FindNodes("//p[@data-test='subject-name']"))
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
        name = name
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref($"/{name}");

        //https://www.freeones.com/Ariel-Rebel
        return result;
    }

    protected override Task HandlePersonEntry(PageDocument girlPage, CancellationToken token)
    {
        //Pic
        girlPage
            .FindNodes("//div[@class='dashboard-image-container ']/a/img")
            .Select(x => x.GetSrc())
            .OfType<PageUri>()
            .ToList()
            .ForEach(x => UpdateProfilePictures(x));

        //Personal
        var dataNodes = girlPage.FindNodes("//ul[@class='profile-meta-list  ']/li");
        dataNodes.AddRange(girlPage.FindNodes("//ul[@class='profile-meta-list ']/li"));

        int? bust = null;
        string? boobs = null;
        int? waist = null;
        int? hip = null;

        foreach (var dataNode in dataNodes)
        {

            var desc = dataNode.FindNode("./span")?.GetText();
            var valueNode = dataNode.FindNode("./span[2]");

            var text = valueNode?
                .GetText()
                .Except("Unknown")
                .Trim();
            text = string.IsNullOrWhiteSpace(text) ? null : text;

            Action act = desc switch
            {
                "Name:" => () => UpdateName(valueNode),
                "Aliases:" => () => Result.Aliases.AddRange(text.SplitList()),
                "Official website:" => () => UpdateSocial(valueNode?.FindNode("./a")?.GetHref()?.WithoutQuery(), SocialLinkKind.Official),
                "Date of birth:" => () => Result.Birthday = text.ToDateOnly(),
                "Profession:" => () => Result.Profession = string.Join(',', text.SplitList()),
                "Career status:" => () => Result.StillActive = PersonParser.FindStillActiveInText(text),
                "Career start:" => () => Result.CareerStart = PersonParser.FindCareerStartInText(text),
                "Place of birth:" => () => Result.City = text.SplitList().FirstOrDefault(),
                "Nationality:" => () => UpdateCountry(text),

                "Ethnicity:" => () => Result.Ethnicity = text,
                "Boobs:" => () => boobs = text,
                "Bust:" => () => bust = int.TryParse(text, out var b) ? b : null,
                "Cup:" => () => UpdateMeasurements(text),
                "Waist:" => () => waist = PersonParser.FindHeightInText(text),
                "Hip:" => () => hip = PersonParser.FindHeightInText(text),
                "Height:" => () => Result.Height = PersonParser.FindHeightInText(text),
                "Weight:" => () => Result.Weight = PersonParser.FindWeightInText(text),
                "Hair Color:" => () => Result.HairColor = text,
                "Eye Color:" => () => Result.EyeColor = text,
                "Piercing locations:" => () => Result.Piercings = text == "No" ? null : text,

                _ => () => { }
            };
            act();
        }

        UpdateMeasurements($"{bust}-{waist}-{hip}");
        if (!string.IsNullOrWhiteSpace(Result.MeasurementDetails.Cup))
            UpdateMeasurements($"{Result.MeasurementDetails.Cup}({boobs})");

        return Task.CompletedTask;
    }
}
