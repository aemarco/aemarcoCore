namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("IStripper", 40)]
internal class Stripper : PersonCrawlerBase
{

    public Stripper(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://www.istripper.com"))
    { }


    protected override string GetSiteHref()
    {
        // de/models/Aletta-Ocean
        var href = $"de/models/{NameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@class='trigger']/div/h1");
        AddNameFromInnerText(nameNode);

        //Picture
        var picNode = document.DocumentNode.SelectSingleNode("//div[@class='container']/img");
        AddProfilePicture(picNode, "src",
            suggestedMinAdultLevel: 35,
            suggestedMaxAdultLevel: 39);

        //Data
        var nodeWithData = document.DocumentNode.SelectSingleNode("//ul[@class='info2']");
        if (nodeWithData is null)
            return Task.CompletedTask;

        Result.Gender = Gender.Female;

        foreach (var node in nodeWithData.ChildNodes
                     .Where(x => !string.IsNullOrWhiteSpace(x.InnerText)))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nodeText = GetInnerText(node);

            if (node.InnerText.Contains("Land:"))
                Result.Country = GetInnerText(node, removals: "Land:");
            else if (node.InnerText.Contains("Stadt:"))
                Result.City = GetInnerText(node, removals: "Stadt:");
            else if (node.InnerText.Contains("Größe"))
                Result.Height = FindHeightInText(nodeText);
            else if (node.InnerText.Contains("Gewicht:"))
                Result.Weight = FindWeightInText(nodeText);
            else if (node.InnerText.Contains("Maße:"))
                UpdateFromMeasurementsText(nodeText);
        }

        return Task.CompletedTask;

    }


}