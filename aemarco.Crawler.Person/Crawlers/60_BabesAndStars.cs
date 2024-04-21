namespace aemarco.Crawler.Person.Crawlers;

[Crawler("BabesAndStars", 60)]
internal class BabesAndStars : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.babesandstars.com");

    protected override PageUri GetGirlUri(string firstName, string lastName)
    {
        var name = $"{firstName}-{lastName}"
            .Replace(' ', '-')
            .ToLower();
        var result = new PageUri(_uri).WithHref($"/{name[0]}/{name}/");

        //https://www.babesandstars.com/a/ariel-rebel/
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        var profileNode = girlPage.FindNode("//div[@class='profile']/div");

        //Name
        UpdateName(profileNode?.FindNode("./div/h1"));

        //Pic
        UpdateProfilePictures(profileNode?
            .FindNode("./div[@class='thumb']/img")?
            .GetSrc());

        //Rating
        Result.Rating = PersonParser.FindRatingInText(profileNode?
            .FindNode("./div[@class='info']/form/em")?
            .GetText());

        //Data
        var infoNode = profileNode?.FindNode("./div[@class='info']/div[@class='middle']");
        if (infoNode is null)
            return Task.CompletedTask;

        var aliasNode = infoNode.FindNode("./div[@class='aliases']/em");
        var aliasText = aliasNode?.GetText().TitleCase();
        Result.Aliases.AddRange(aliasText.SplitList());

        var dataNodes = infoNode.FindNodes("./div[@class='features']/span");
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            var text = node.GetText().TitleCase();
            if (text.StartsWith("Country:"))
                Result.Country = text.Except("Country:");
            else if (text.StartsWith("Ethnicity:"))
                Result.Ethnicity = text.Except("Ethnicity:");
            else if (text.StartsWith("Measurements:"))
                UpdateMeasurements(text.Except("Measurements:"), true);
            else if (text.StartsWith("Birthday:"))
                Result.Birthday = text.Except("Birthday:").ToDateOnly();
            else if (text.StartsWith("Eyes:"))
                Result.EyeColor = text.Except("Eyes:");
            else if (text.StartsWith("Cup:"))
                UpdateMeasurements(text.Except("Cup:"), true);
            else if (text.StartsWith("Weight:"))
                Result.Weight = PersonParser.FindWeightInText(text);
            else if (text.StartsWith("Height:"))
                Result.Height = PersonParser.FindHeightInText(text);
            else if (text.StartsWith("Hair:"))
                Result.HairColor = text.Except("Hair:");
        }
        return Task.CompletedTask;

    }

}