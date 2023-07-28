namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("IStripper", 50)]
internal class Stripper : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.istripper.com");


    //z.B. "https://www.istripper.com/de/models/Aletta-Ocean"
    protected override PageUri GetGirlUri(string nameToCrawl) =>
        new PageUri(_uri).WithHref($"/de/models/{nameToCrawl.Replace(' ', '-')}");
    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {

        //Name
        UpdateName(girlPage.FindNode("//div[@class='trigger']/div/h1"));

        //Pic
        var picUri = girlPage
            .FindNode("//div[@class='container']/img")?
            .GetSrc();
        UpdateProfilePictures(picUri, 35, 39);

        //Data
        var dataNodes = girlPage.FindNodes("//ul[@class='info2']/li");
        Result.Gender = Gender.Female; //always female on this site
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();
            var text = node.GetText();

            if (text.Contains("Land:"))
                Result.Country = text.Except("Land:").TitleCase();
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