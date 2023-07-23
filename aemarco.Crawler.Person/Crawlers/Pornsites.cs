namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Pornsites", 40)]
internal class Pornsites : PersonCrawlerBase
{

    public Pornsites()
        : base(new Uri("https://pornsites.xxx"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // pornstars/Aletta-Ocean
        var href = $"pornstars/{nameToCrawl.Replace(' ', '-')}";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {

        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//div[@id='main']/header/h1");
        UpdateName(nameNode);

        //Pictures
        var picNodes = document.DocumentNode
            .SelectNodes("//div[@class='pornstar-box-con-big']/div[@class='pornstar-box']/picture/img");
        UpdateProfilePictures(picNodes, "src");


        //Data
        var nodeWithData = document.DocumentNode.SelectNodes("//table[@class='styled']/tr");
        if (nodeWithData is null)
            return Task.CompletedTask;


        foreach (var node in nodeWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nodeText = node.TextWithout();


            //Geburtstag
            if (nodeText.StartsWith("Age:"))
                Result.Birthday = DataParser.FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Gender:"))
                Result.Gender = DataParser.FindGenderInText(nodeText); //female male trans
            else if (nodeText.StartsWith("Hair Color:"))
                Result.HairColor = node.TextWithout("Hair Color:");
            else if (nodeText.StartsWith("Eye Color:"))
                Result.EyeColor = node.TextWithout("Eye Color:");
            else if (nodeText.StartsWith("Cupsize:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Weight:"))
                Result.Weight = DataParser.FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Height:"))
                Result.Height = DataParser.FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Country:"))
                Result.Country = node.TextWithout("Country:");
            else if (nodeText.StartsWith("Ethnicity:"))
                Result.Ethnicity = node.TextWithout("Ethnicity:");
        }
        return Task.CompletedTask;
    }

}