namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Babepedia", 1)]
internal class Babepedia : PersonCrawlerBase
{
    public Babepedia(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://www.babepedia.com"))
    { }


    protected override string GetSiteHref()
    {
        // /babe/Chloe_Temple
        var href = $"/babe/{NameToCrawl.Replace(' ', '_')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@id='bioarea']/h1");
        AddNameFromInnerText(nameNode);

        //Pictures
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@id='profimg']/a[@class='img']");
        AddProfilePicture(picNode, "href", UrlFromHref);
        var addPicNodes = document.DocumentNode
            .SelectNodes("//div[@id='profselect']/div[@class='prof']/a[@class='img']");
        AddProfilePictures(addPicNodes, "href", UrlFromHref);

        //Aliases
        var nodeWithAlias = document.DocumentNode
            .SelectSingleNode("//div[@id='bioarea']/h2");
        if (nodeWithAlias is not null)
        {
            var cleaned = GetInnerText(nodeWithAlias).Replace("aka ", string.Empty, StringComparison.OrdinalIgnoreCase);
            Result.Aliases = GetListFromCsv(cleaned, '/');
        }

        //Data
        var nodeWithData = document.DocumentNode
            .SelectSingleNode("//div[@id='bioarea']/ul");
        if (nodeWithData is null)
            return Task.CompletedTask;

        foreach (var node in nodeWithData.ChildNodes
                     .Where(x => x.Name == "li"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nodeText = GetInnerText(node);


            if (nodeText.StartsWith("Born:"))
                Result.Birthday = FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Birthplace:"))
            {
                var str = GetInnerText(node, removals: "Birthplace:");
                var parts = GetListFromCsv(str, ',');
                switch (parts.Count)
                {
                    case 1:
                        Result.Country = parts[0];
                        break;
                    case 2:
                        Result.City = parts[0];
                        Result.Country = parts[1];
                        break;
                }
            }
            else if (nodeText.StartsWith("Profession:"))
                Result.Profession = GetInnerText(node, removals: "Profession:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = GetInnerText(node, removals: "Ethnicity:");
            else if (nodeText.StartsWith("Hair Color:"))
                Result.HairColor = GetInnerText(node, removals: "Hair Color:");
            else if (nodeText.StartsWith("Eye Color:"))
                Result.EyeColor = GetInnerText(node, removals: "Eye Color:");
            else if (nodeText.StartsWith("Measurements:"))
                UpdateFromMeasurementsText(nodeText, true);
            else if (nodeText.StartsWith("Bra/Cup Size:"))
                UpdateFromCupText(GetInnerText(node, removals: "Bra/Cup Size:"));
            else if (nodeText.StartsWith("Height:"))
                Result.Height = FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Years Active:"))
            {
                Result.CareerStart = FindCareerStartInText(nodeText);
                Result.StillActive = FindStillActiveInText(nodeText);
            }
            else if (nodeText.StartsWith("Piercings:"))
                Result.Piercings = GetInnerText(node, removals: "Piercings:");


        }

        return Task.CompletedTask;
    }

}