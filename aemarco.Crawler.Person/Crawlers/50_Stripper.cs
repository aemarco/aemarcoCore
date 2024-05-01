namespace aemarco.Crawler.Person.Crawlers;

[Crawler("IStripper", 50)]
internal class Stripper : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.istripper.com");

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/de/models/{name}"
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://www.istripper.com/de/models/Aletta-Ocean
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {

        //Name
        UpdateName(girlPage.FindNode("//div[@class='trigger']/div/h1"));

        //Pic
        var picUri = girlPage
            .FindNode("//div[@class='container']/img")?
            .GetSrc();
        UpdateProfilePictures(picUri, 35, 39);

        //Rating

        if (PersonParser.FindRatingInText(girlPage.FindNode("//span[@class='star rate']")?.GetText()) is { } rating)
            Result.Rating = rating * 2;

        //Data
        var dataNodes = girlPage.FindNodes("//ul[@class='info2']/li");
        Result.Gender = Gender.Female; //always female on this site
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();
            var text = node.GetText();

            if (text.Contains("Land:"))
                UpdateCountry(text.Except("Land:").TitleCase());
            else if (text.Contains("Stadt:"))
                Result.City = text.Except("Stadt:").TitleCase();
            else if (text.Contains("Größe"))
                Result.Height = PersonParser.FindHeightInText(text.Except("Größe").TitleCase());
            else if (text.Contains("Gewicht:"))
                Result.Weight = PersonParser.FindWeightInText(text.Except("Gewicht:").TitleCase());
            else if (text.Contains("Maße:"))
                UpdateMeasurements(text.Except("Maße:").TitleCase());
        }
        return Task.CompletedTask;

    }

}