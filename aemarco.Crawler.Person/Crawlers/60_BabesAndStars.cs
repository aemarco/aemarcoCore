namespace aemarco.Crawler.Person.Crawlers;

[Crawler("BabesAndStars", 60)]
internal class BabesAndStars : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.babesandstars.com");


    protected override async Task<PersonNameInfo[]> HandlePersonNameEntries(CancellationToken token)
    {
        var uri = new PageUri(_uri).WithHref("/top-models/");
        var page = await uri.NavigateAsync(token: token);
        List<PersonNameInfo> result = [];

        //first page of performers
        // order: rating desc

        foreach (var node in page.FindNodes("//div[@class='item']/a/span"))
        {
            var text = node.GetText().TitleCase();
            var (fn, ln) = PersonParser.FindNameInText(text);
            if (fn is null || ln is null)
                continue;

            result.Add(new PersonNameInfo(fn, ln));
        }


        return [.. result.Distinct()];
    }

    protected override PageUri GetGirlUri(string name)
    {
        name = name
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
        if (profileNode?.FindNode("./div[@class='info']/form/em")?.GetText() is { } ratingText &&
            PersonParser.FindRatingInText(ratingText) is { } rating)
            Result.Rating = rating;

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
                UpdateCountry(text.Except("Country:"));
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