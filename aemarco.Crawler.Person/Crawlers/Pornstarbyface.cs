namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Pornstarbyface", 10)]
internal class Pornstarbyface : PersonCrawlerBase
{

    public Pornstarbyface()
        : base(new Uri("https://pornstarbyface.com/"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // /girls/Ariel-Rebel
        var href = $"/girls/{nameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode
            .SelectSingleNode("//div[@class='star-info']/h5");
        UpdateName(nameNode);


        //Picture
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@class='col-lg-3 profile-image']/img[@class='img-responsive']");
        UpdateProfilePictures(picNode, "src");


        //Data
        var nodesWithData = document.DocumentNode
            .SelectNodes("//div[@class='star-info']/div/b");
        if (nodesWithData is null)
            return Task.CompletedTask;

        Result.Gender = Gender.Female;

        foreach (var node in nodesWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desc = node.InnerText.Trim();
            if (string.IsNullOrWhiteSpace(desc))
                continue;

            if (node.ParentNode.NextSibling is not { } valueNode)
                continue;
            var valueNodeText = valueNode.TextWithout();

            Action act = desc switch
            {
                "Aliases" => () => Result.Aliases = DataParser.FindStringsInText(valueNodeText),
                "Country" => () => Result.Country = valueNodeText,
                "State" => () => Result.City = valueNodeText,
                "Birthday" => () => Result.Birthday = DataParser.FindBirthdayInText(valueNodeText),
                "Ethnicity" => () => Result.Ethnicity = valueNodeText,
                "Eye" => () => Result.EyeColor = valueNodeText,
                "Hair" => () => Result.HairColor = valueNodeText,
                "Height" => () => Result.Height = DataParser.FindHeightInText(valueNodeText),
                "Weight" => () => Result.Weight = DataParser.FindWeightInText(valueNodeText),
                "Measurements" => () => UpdateMeasurements(valueNodeText, true),
                "Cup" => () => UpdateMeasurements(valueNodeText),
                _ => () => { }
            };
            act();
        }
        return Task.CompletedTask;
    }

}