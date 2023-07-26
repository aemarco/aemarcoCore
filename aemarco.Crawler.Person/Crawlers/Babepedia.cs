namespace aemarco.Crawler.Person.Crawlers;


//https://www.babepedia.com/babe/Chloe_Temple
//https://web.archive.org/web/20230528093234/https://www.babepedia.com/babe/Chloe_Temple

//https://www.zenrows.com/blog/bypass-cloudflare#how-cloudflare-detects-bots

[Obsolete]
[PersonCrawler("Babepedia", 20)]
internal class Babepedia : PersonCrawlerBase
{

    public Babepedia()
        : base(new Uri("https://www.babepedia.com"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // /babe/Chloe_Temple
        var href = $"/babe/{nameToCrawl.Replace(' ', '_')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {
        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@id='bioarea']/h1");
        UpdateName(nameNode);

        //Pictures
        var picNode = document.DocumentNode
            .SelectSingleNode("//div[@id='profimg']/a[@class='img']");
        UpdateProfilePictures(picNode, "href");
        var addPicNodes = document.DocumentNode
            .SelectNodes("//div[@id='profselect']/div[@class='prof']/a[@class='img']");
        UpdateProfilePictures(addPicNodes, "href");

        //Aliases
        var nodeWithAlias = document.DocumentNode
            .SelectSingleNode("//div[@id='bioarea']/h2");
        if (nodeWithAlias is not null)
        {
            var cleaned = nodeWithAlias.TextWithout().TextWithoutBeginning("aka");
            Result.Aliases = DataParser.FindStringsInText(cleaned, '/');
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

            var nodeText = node.TextWithout();


            if (nodeText.StartsWith("Born:"))
                Result.Birthday = DataParser.FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Birthplace:"))
            {
                var str = node.TextWithout("Birthplace:");
                var parts = DataParser.FindStringsInText(str);
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
                Result.Profession = node.TextWithout("Profession:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = node.TextWithout("Ethnicity:");
            else if (nodeText.StartsWith("Hair Color:"))
                Result.HairColor = node.TextWithout("Hair Color:");
            else if (nodeText.StartsWith("Eye Color:"))
                Result.EyeColor = node.TextWithout("Eye Color:");
            else if (nodeText.StartsWith("Measurements:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Bra/Cup Size:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Height:"))
                Result.Height = DataParser.FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = DataParser.FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Years Active:"))
            {
                Result.CareerStart = DataParser.FindCareerStartInText(nodeText);
                Result.StillActive = DataParser.FindStillActiveInText(nodeText);
            }
            else if (nodeText.StartsWith("Piercings:"))
                Result.Piercings = node.TextWithout("Piercings:");
        }
        return Task.CompletedTask;
    }

}