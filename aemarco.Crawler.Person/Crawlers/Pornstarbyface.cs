namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Pornstarbyface", 10)]
internal class Pornstarbyface : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://pornstarbyface.com");


    //z.B. "https://pornstarbyface.com/girls/Aletta-Ocean"
    protected override PageUri GetGirlUri(string nameToCrawl) =>
        new PageUri(_uri).WithHref($"/girls/{nameToCrawl.Replace(' ', '-')}");
    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        var starInfo = girlPage.FindNode("//div[@class='star-info']");
        if (starInfo is null)
            return Task.CompletedTask;

        //Name
        UpdateName(starInfo.FindNode("./h5"));

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
                "Country" => () => Result.Country = text,
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