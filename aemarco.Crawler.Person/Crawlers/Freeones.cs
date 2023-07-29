namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Freeones", 12)]
internal class Freeones : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.freeones.com");

    protected override PageUri GetGirlUri(string nameToCrawl) =>
        new PageUri(_uri).WithHref($"/{nameToCrawl.Replace(' ', '-').ToLower()}");

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
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
                "Nationality:" => () => Result.Country = text,

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
                "Piercing locations:" => () => Result.Piercings = text,

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
