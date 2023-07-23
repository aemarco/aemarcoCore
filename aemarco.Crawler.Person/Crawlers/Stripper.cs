namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("IStripper", 50)]
internal class Stripper : PersonCrawlerBase
{

    public Stripper()
        : base(new Uri("https://www.istripper.com"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // de/models/Aletta-Ocean
        var href = $"de/models/{nameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@class='trigger']/div/h1");
        UpdateName(nameNode);

        //Picture
        var picNode = document.DocumentNode.SelectSingleNode("//div[@class='container']/img");
        UpdateProfilePictures(picNode, "src",
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
            var nodeText = node.TextWithout();

            if (node.InnerText.Contains("Land:"))
                Result.Country = node.TextWithout("Land:");
            else if (node.InnerText.Contains("Stadt:"))
                Result.City = node.TextWithout("Stadt:");
            else if (node.InnerText.Contains("Größe"))
                Result.Height = DataParser.FindHeightInText(nodeText);
            else if (node.InnerText.Contains("Gewicht:"))
                Result.Weight = DataParser.FindWeightInText(nodeText);
            else if (node.InnerText.Contains("Maße:"))
                UpdateMeasurements(nodeText);
        }
        return Task.CompletedTask;
    }

}