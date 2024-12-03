namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Milffox", 35)]
internal class Milffox : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.milffox.com");

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/milf-pornstars/{name}/"
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://www.milffox.com/milf-pornstars/Carolina-Sweets/
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {

        if (girlPage.FindNode("//div[@class='main_wrapper']/div/div[@class='header']/span") is not { } header ||
            header.GetText() is not { } headerText ||
            !headerText.StartsWith("Pornstar Profile: "))
            return Task.CompletedTask;

        UpdateName(headerText[18..]);

        UpdateProfilePictures(
            girlPage.FindNode("//div[@id='review']/div/div[@class='ps-img']/a/img")?
                .GetSrc());

        var dataNodes = girlPage.FindNodes("//div[@id='review']/div/div/div[@class='row']");
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            var label = node.FindNode("./strong")?.GetText();
            var text = node.FindNode("./span")?.GetText();
            if (string.IsNullOrWhiteSpace(text) || text == "Unknown")
                continue;

            Action act = label switch
            {
                "Birthday:" => () => Result.Birthday = text.ToDateOnly(),
                "Place of Birth:" => () =>
                {
                    var parts = text.SplitList().ToList();
                    switch (parts.Count)
                    {
                        case 1:
                            UpdateCountry(parts[0]);
                            break;
                        case 2:
                            Result.City = parts[0];
                            UpdateCountry(parts[1]);
                            break;
                        case 3:
                            Result.City = parts[0];
                            UpdateCountry(parts[2]);
                            break;
                    }
                }
                ,
                "Ethnicity:" => () => Result.Ethnicity = text,
                "Career:" => () => Result.StillActive = PersonParser.FindStillActiveInText(text),
                "Career Start/End:" => () => Result.CareerStart = PersonParser.FindCareerStartInText(text),
                "Aka:" => () => Result.Aliases.AddRange(text.SplitList(',')),
                "Social Network:" => () => node.FindNodes("./span/span/a").ForEach(linkNode => UpdateSocial(linkNode.GetHref())),

                "Measurements:" => () => UpdateMeasurements(text),
                "Weight:" => () => Result.Weight = PersonParser.FindWeightInText(text),
                "Height:" => () => Result.Height = PersonParser.FindHeightInText(text),
                "Eye Color:" => () => Result.EyeColor = text,
                "Hair Color:" => () => Result.HairColor = text,
                "Piercings:" => () => Result.Piercings = text,
                "Fake Tits?" => () => Result.MeasurementDetails = Result.MeasurementDetails.Combine(new MeasurementDetails(null, null, text.Contains("Yes", StringComparison.OrdinalIgnoreCase), null, null)),
                _ => () => { }
            };
            act();

            //ignores:
            // Home:
            // Nationality:
            // Tattoos:


        }
        return Task.CompletedTask;
    }
}
