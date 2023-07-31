namespace aemarco.Crawler.Person.Crawlers;


//https://www.babepedia.com/babe/Chloe_Temple
//https://web.archive.org/web/20230528093234/https://www.babepedia.com/babe/Chloe_Temple

//https://www.zenrows.com/blog/bypass-cloudflare#how-cloudflare-detects-bots

[Obsolete]
[Crawler("Babepedia", 20)]
internal class Babepedia : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.babepedia.com");


    //z.B. "https://www.babepedia.com/babe/Chloe_Temple/"
    protected override PageUri GetGirlUri(string firstName, string lastName) =>
        new PageUri(_uri).WithHref($"/babe/{firstName.Replace(' ', '_')}_{lastName.Replace(' ', '_')}");
    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {

        //Name
        UpdateName(girlPage.FindNode("//div[@id='bioarea']/h1"));

        //Pics
        UpdateProfilePictures(girlPage
            .FindNode("//div[@id='profimg']/a[@class='img']")?
            .GetHref());
        girlPage
            .FindNodes("//div[@id='profselect']/div[@class='prof']/a[@class='img']")
            .Select(x => x.GetHref())
            .ToList()
            .ForEach(x => UpdateProfilePictures(x));

        //Aliases
        var aliasNode = girlPage.FindNode("//div[@id='bioarea']/h2");
        if (aliasNode is not null)
        {
            var cleaned = aliasNode.GetText().TitleCase();
            cleaned = cleaned.StartsWith("Aka") ? cleaned[3..] : cleaned;
            Result.Aliases.AddRange(cleaned.SplitList('/'));
        }

        //TODO Babepedia add rating
        //TODO Babepedia add socials

        //Data
        var dataNodes = girlPage.FindNodes("//div[@id='bioarea']/ul/li");
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            var text = node.GetText().TitleCase();
            if (text.StartsWith("Born:"))
                Result.Birthday = text.Except("Born:").ToDateOnly();
            else if (text.StartsWith("Birthplace:"))
            {
                var str = text.Except("Birthplace:");
                var parts = str.SplitList().ToList();
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
            else if (text.StartsWith("Profession:"))
                Result.Profession = text.Except("Profession:");
            else if (text.StartsWith("Ethnicity:"))
                Result.Ethnicity = text.Except("Ethnicity:");
            else if (text.StartsWith("Hair Color:"))
                Result.HairColor = text.Except("Hair Color:");
            else if (text.StartsWith("Eye Color:"))
                Result.EyeColor = text.Except("Eye Color:");
            else if (text.StartsWith("Height:"))
                Result.Height = PersonParser.FindHeightInText(text);
            else if (text.StartsWith("Weight:"))
                Result.Weight = PersonParser.FindWeightInText(text);
            else if (text.StartsWith("Measurements:"))
                UpdateMeasurements(text.Except("Measurements:"), true);
            else if (text.StartsWith("Bra/Cup Size:"))
                UpdateMeasurements(text.Except("Bra/Cup Size:"), true);
            else if (text.StartsWith("Boobs:"))
                Result.MeasurementDetails.Combine(new MeasurementDetails(null, null, text.Except("Boobs:").Contains("fake", StringComparison.OrdinalIgnoreCase), null, null));
            else if (text.StartsWith("Years Active:"))
            {
                Result.CareerStart = PersonParser.FindCareerStartInText(text.Except("Years Active:"));
                Result.StillActive = PersonParser.FindStillActiveInText(text.Except("Years Active:"));
            }
            else if (text.StartsWith("Piercings:"))
                Result.Piercings = text.Except("Piercings:");
        }
        return Task.CompletedTask;
    }

}