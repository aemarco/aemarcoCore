namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Pornstarbyface", 0)]
internal class Pornstarbyface : PersonCrawlerBase
{
    public Pornstarbyface(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://pornstarbyface.com/"))
    { }


    protected override string GetSiteHref()
    {
        // /girls/Ariel-Rebel
        var href = $"/girls/{NameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode
            .SelectSingleNode("//div[@class='star-info']/h5");
        AddNameFromInnerText(nameNode);


        //Picture
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@class='col-lg-3 profile-image']/img[@class='img-responsive']");
        AddProfilePicture(picNode, "src", UrlFromHref);


        //Data
        var nodesWithData = document.DocumentNode
            .SelectNodes("//div[@class='star-info']/div/b");
        if (nodesWithData is null)
            return Task.CompletedTask;
        foreach (var node in nodesWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desc = node.InnerText.Trim();
            if (string.IsNullOrWhiteSpace(desc))
                continue;
            if (node.ParentNode.NextSibling is not { } valueNode)
                continue;
            var valueNodeText = GetInnerText(valueNode);


            Action act = desc switch
            {
                "Aliases" => () => Result.Aliases = GetListFromCsv(valueNodeText, ','),
                "Country" => () => Result.Country = valueNodeText,
                "State" => () => Result.City = valueNodeText,
                "Birthday" => () => Result.Birthday = FindBirthdayInText(valueNodeText),
                "Ethnicity" => () => Result.Ethnicity = valueNodeText,
                "Eye" => () => Result.EyeColor = valueNodeText,
                "Hair" => () => Result.HairColor = valueNodeText,
                "Height" => () => Result.Height = FindHeightInText(valueNodeText),
                "Weight" => () => Result.Weight = FindWeightInText(valueNodeText),
                "Measurements" => () => UpdateFromMeasurementsText(valueNodeText, true),
                "Cup" => () => UpdateFromCupText(valueNodeText),
                _ => () => { }
            };
            act();
        }
        return Task.CompletedTask;
    }
}