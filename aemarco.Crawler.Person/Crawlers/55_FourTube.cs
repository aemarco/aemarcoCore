namespace aemarco.Crawler.Person.Crawlers;

[Crawler("4Tube", 55)]
internal class FourTube : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.4tube.com");

    protected override PageUri GetGirlUri(string name)
    {
        name = name
            .Replace(' ', '-')
            .ToLower();
        var result = new PageUri(_uri)
            .WithHref($"/pornstars/{name}");

        //https://www.4tube.com/pornstars/foxy-di
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        var profile = girlPage.FindNode("//div[@class='pornstar-profile container']");
        if (profile is null)
            return Task.CompletedTask;

        //Name
        UpdateName(girlPage
            .FindNode("//span[@itemprop='name']"));
        //Pics
        UpdateProfilePictures(girlPage
            .FindNode("//div[@class='photo']/img")?
            .GetSrc());

        //Aliases
        var aliasNode = girlPage.FindNode("//p[@itemprop='alternateName']");
        if (aliasNode?.GetText().TextInParentheses() is { } aliasText)
        {
            var cleaned = aliasText.TitleCase();
            Result.Aliases.AddRange(cleaned.SplitList(','));
        }

        //Data
        var dataNodes = girlPage.FindNodes("//dl[@class='data']/dt");
        foreach (var node in dataNodes)
        {
            var text = node.GetText();
            var info = node.NextSibling().GetText();

            Action act = text switch
            {
                "Measurements" => () => UpdateMeasurements(info, true),
                "Height" => () => Result.Height = info.NumberInText(),
                "Date of Birth" => () => Result.Birthday = info.ToDateOnly(),
                _ => () => { }
            };
            act();
        }

        //meta
        var metaNodes = girlPage.FindNodes("//div[@class='scrolly']/meta");
        foreach (var node in metaNodes)
        {
            var text = node.GetAttribute("itemprop");
            var info = node.GetAttribute("content");

            Action act = text switch
            {
                "gender" => () => Result.Gender = PersonParser.FindGenderInText(info),
                "nationality" => () => UpdateCountry(info),
                "jobTitle" => () => Result.Profession = info,
                _ => () => { }
            };
            act();
        }

        return Task.CompletedTask;
    }
}
