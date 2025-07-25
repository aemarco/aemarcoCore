namespace aemarco.Crawler.Person.Crawlers;


//https://www.babepedia.com/babe/Chloe_Temple
//https://web.archive.org/web/20230528093234/https://www.babepedia.com/babe/Chloe_Temple

//https://www.zenrows.com/blog/bypass-cloudflare#how-cloudflare-detects-bots


[Crawler("Babepedia", 20)]
internal class Babepedia : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.babepedia.com");

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/babe/{name}"
            .Replace(' ', '_');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://www.babepedia.com/babe/Chloe_Temple
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        //Pics
        //no pics, because they are not accessible later on

        //UpdateProfilePictures(girlPage
        //    .FindNode("//div[@id='profimg']/a[@class='img']")?
        //    .GetHref());
        //girlPage
        //    .FindNodes("//div[@id='profselect']/div[@class='prof']/a[@class='img']")
        //    .Select(x => x.GetHref())
        //    .ToList()
        //    .ForEach(x => UpdateProfilePictures(x));


        //TODO Babepedia add rating
        //TODO Babepedia add socials



        //Name
        if (girlPage.FindNode("//div[@id='name-block']/h1[@id='babename']") is { } nameNode)
            UpdateName(nameNode.GetText());

        //Aliases
        foreach (var alias in girlPage
                     .FindNodes("//span[@class='aliasname']")
                     .Select(x => x.GetText().TitleCase())
                     .Distinct())
        {
            token.ThrowIfCancellationRequested();

            Result.Aliases.Add(alias);
        }

        var data = girlPage.FindNodes("//div[@class='info-grid']/div[@class='info-item']");
        foreach (var dataNode in data)
        {
            token.ThrowIfCancellationRequested();

            var label = dataNode.FindNode("./span[@class='label']")?.GetText().TitleCase();
            var value = dataNode.FindNode("./span[@class='value']")?.GetText().TitleCase();
            if (string.IsNullOrWhiteSpace(label) ||
                string.IsNullOrWhiteSpace(value))
                continue;

            Action act = label switch
            {
                "Born:" => () => Result.Birthday = value.ToDateOnly(),
                "Years Active:" => () =>
                {
                    Result.CareerStart = PersonParser.FindCareerStartInText(value);
                    Result.StillActive = PersonParser.FindStillActiveInText(value);
                }
                ,
                "Birthplace:" => () =>
                {
                    var parts = value.SplitList().ToList();
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
                "Ethnicity:" => () => Result.Ethnicity = value,
                "Professions:" => () => Result.Profession = value,
                "Hair Color:" => () => Result.HairColor = value,
                "Eye Color:" => () => Result.EyeColor = value,
                "Height:" => () => Result.Height = PersonParser.FindHeightInText(value),
                "Weight:" => () => Result.Weight = PersonParser.FindWeightInText(value),
                "Measurements:" => () => UpdateMeasurements(value, true),
                "Bra/Cup Size:" => () => UpdateMeasurements(value, true),
                "Boobs:" => () => UpdateMeasurements(new MeasurementDetails(null, null, value.Contains("fake", StringComparison.OrdinalIgnoreCase), null, null)),
                "Piercings:" => () => Result.Piercings = value,
                _ => () => { }
            };
            act();
        }

        return Task.CompletedTask;
    }

}