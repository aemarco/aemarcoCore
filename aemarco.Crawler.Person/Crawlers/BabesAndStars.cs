namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("BabesAndStars", 60)]
internal class BabesAndStars : PersonCrawlerBase
{

    public BabesAndStars()
        : base(new Uri("https://www.babesandstars.com/"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // /babe/Chloe_Temple
        var href = $"/{nameToCrawl[0].ToString().ToLower()}/{nameToCrawl.Replace(' ', '-').ToLower()}/";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@class='profile']/div/div/h1");
        UpdateName(nameNode);

        //Pictures
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@class='profile']/div/div[@class='thumb']/img");
        UpdateProfilePictures(picNode, "src");

        //Aliases
        var nodeWithAlias = document.DocumentNode
            .SelectSingleNode("//div[@class='profile']/div/div[@class='info']/div[@class='middle']/div[@class='aliases']/em");
        if (nodeWithAlias is not null)
        {
            var cleaned = nodeWithAlias.TextWithout();
            Result.Aliases = DataParser.FindStringsInText(cleaned);
        }

        //Data
        var nodeWithData = document.DocumentNode
            .SelectSingleNode("//div[@class='profile']/div/div[@class='info']/div[@class='middle']/div[@class='features']");
        if (nodeWithData is null)
            return Task.CompletedTask;

        foreach (var node in nodeWithData.ChildNodes
                     .Where(x => x.Name == "span"))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeText = node.TextWithout();

            if (nodeText.StartsWith("Country:"))
                Result.Country = node.TextWithout("Country:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = node.TextWithout("Ethnicity:");
            else if (nodeText.StartsWith("Measurements:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Birthday:"))
                Result.Birthday = DataParser.FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Eyes:"))
                Result.EyeColor = node.TextWithout("Eyes:");
            else if (nodeText.StartsWith("Cup:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = DataParser.FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Height:"))
                Result.Height = DataParser.FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Hair:"))
                Result.HairColor = node.TextWithout("Hair:");
        }
        return Task.CompletedTask;
    }

}