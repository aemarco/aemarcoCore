namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("BabesAndStars", 50)]
internal class BabesAndStars : PersonCrawlerBase
{
    public BabesAndStars(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://www.babesandstars.com/"))
    {
    }

    protected override string GetSiteHref()
    {
        // /babe/Chloe_Temple
        var href = $"/{NameToCrawl[0].ToString().ToLower()}/{NameToCrawl.Replace(' ', '-').ToLower()}/";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@class='profile']/div/div/h1");
        AddNameFromInnerText(nameNode);

        //Pictures
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@class='profile']/div/div[@class='thumb']/img");
        AddProfilePicture(picNode, "src");

        //Aliases
        var nodeWithAlias = document.DocumentNode
            .SelectSingleNode("//div[@class='profile']/div/div[@class='info']/div[@class='middle']/div[@class='aliases']/em");
        if (nodeWithAlias is not null)
        {
            Result.Aliases = GetListFromCsv(GetInnerText(nodeWithAlias), ',');
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
            var nodeText = GetInnerText(node);



            if (nodeText.StartsWith("Country:"))
                Result.Country = GetInnerText(node, removals: "Country:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = GetInnerText(node, removals: "Ethnicity:");
            else if (nodeText.StartsWith("Measurements:"))
                UpdateFromMeasurementsText(nodeText, true);
            else if (nodeText.StartsWith("Birthday:"))
                Result.Birthday = FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Eyes:"))
                Result.EyeColor = GetInnerText(node, removals: "Eyes:");
            else if (nodeText.StartsWith("Cup:"))
                UpdateFromCupText(GetInnerText(node, removals: "Cup:"));
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Height:"))
                Result.Height = FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Hair:"))
                Result.HairColor = GetInnerText(node, removals: "Hair:");




            //else if (nodeText.StartsWith("Birthplace:"))
            //{
            //    var str = GetInnerText(node, removals: "Birthplace:");
            //    var parts = GetListFromCsv(str, ',');
            //    switch (parts.Count)
            //    {
            //        case 1:
            //            Result.Country = parts[0];
            //            break;
            //        case 2:
            //            Result.City = parts[0];
            //            Result.Country = parts[1];
            //            break;
            //    }
            //}
            //else if (nodeText.StartsWith("Profession:"))
            //    Result.Profession = GetInnerText(node, removals: "Profession:");
            //else if (nodeText.StartsWith("Years Active:"))
            //{
            //    Result.CareerStart = FindCareerStartInText(nodeText);
            //    Result.StillActive = FindStillActiveInText(nodeText);
            //}
            //else if (nodeText.StartsWith("Piercings:"))
            //    Result.Piercings = GetInnerText(node, removals: "Piercings:");


        }

        return Task.CompletedTask;
    }
}
