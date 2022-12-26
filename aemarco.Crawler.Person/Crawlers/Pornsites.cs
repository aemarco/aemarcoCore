namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Pornsites", 30)]
internal class Pornsites : PersonCrawlerBase
{
    public Pornsites(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://pornsites.xxx"))
    { }


    protected override string GetSiteHref()
    {
        // pornstars/Aletta-Ocean
        var href = $"pornstars/{NameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {

        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@id='main']/header/h1");
        AddNameFromInnerText(nameNode);

        //Pictures
        var picNodes = document.DocumentNode
            .SelectNodes("//div[@class='pornstar-box-con-big']/div[@class='pornstar-box']/picture/img");
        AddProfilePictures(picNodes, "src");


        //Data
        var nodeWithData = document.DocumentNode.SelectNodes("//table[@class='styled']/tr");
        if (nodeWithData is null)
            return Task.CompletedTask;


        foreach (var node in nodeWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nodeText = GetInnerText(node);


            //Geburtstag
            if (nodeText.StartsWith("Age:"))
                Result.Birthday = FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Hair Color:"))
                Result.HairColor = GetInnerText(node, removals: "Hair Color:");
            else if (nodeText.StartsWith("Eye Color:"))
                Result.EyeColor = GetInnerText(node, removals: "Eye Color:");
            else if (nodeText.StartsWith("Cupsize:"))
                UpdateFromMeasurementsText(nodeText, true);
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Height:"))
                Result.Height = FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Country:"))
                Result.Country = GetInnerText(node, removals: "Country:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = GetInnerText(node, removals: "Ethnicity:");
        }

        return Task.CompletedTask;
    }
}