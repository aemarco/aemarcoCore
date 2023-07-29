namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Nudevista", 30)]
internal class Nudevista : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.nudevista.at");


    //z.B. "https://www.nudevista.at?q=Aletta+Ocean&s=s"
    protected override PageUri GetGirlUri(string nameToCrawl) =>
        new PageUri(_uri).WithHref($"?q={nameToCrawl.Replace(' ', '+')}&s=s");
    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {

        //Name
        UpdateName(girlPage.FindNode("//td[contains(@valign, 'top') and contains(@colspan ,'2')]"));

        //Pic
        UpdateProfilePictures(girlPage
            .FindNode("//img[@class='mthumb']")?
            .GetSrc(x =>
                x.StartsWith("http")
                    ? x :
                    $"https:{x}"));

        //Social
        girlPage
            .FindNodes("//div[@class='msocial']/a")
            .ForEach(x => UpdateSocial(x.GetHref()));

        //Data
        var infoNode = girlPage.FindNode("//div[@id='params_scroll']");
        if (infoNode is null)
            return Task.CompletedTask;

        var aliasNode = infoNode.FindNode("./em");
        if (aliasNode is not null)
        {
            //z.B. "Jessica Kline,   Nikita Charm."
            var aliasString = aliasNode
                .GetText()
                .Except("Auch bekannt als")
                .TitleCase();

            //z.B. "Jessica Kline,   Nikita Charm"
            if (aliasString.EndsWith("."))
                aliasString = aliasString[..^1];

            //z.B. as list "Jessica Kline, Nikita Charm"
            foreach (var aliasItem in aliasString.Split(','))
            {
                var al = aliasItem.Trim();
                if (al.StartsWith(".")) al = al.Remove(0, 1);
                al = al.Trim();

                if (al.Length > 3 && al.Contains(' '))
                {
                    Result.Aliases.Add(al);
                }
            }
        }

        var dataNodes = infoNode.FindNodes("./div");
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            var text = node.GetText().TitleCase();
            if (text.StartsWith("Geschlecht:"))
                Result.Gender = PersonParser.FindGenderInText(text); // female male tranny
            else if (text.StartsWith("Geburtstag:"))
                Result.Birthday = text.Except("Geburtstag:").ToDateOnly();
            else if (text.StartsWith("Land:"))
                Result.Country = text.Except("Land:");
            else if (text.StartsWith("Geburtsort:"))
                Result.City = text.Except("Geburtsort:");
            else if (text.StartsWith("Beruf:"))
                Result.Profession = text.Except("Beruf:");
            else if (text.StartsWith("Karrierestart:"))
                Result.CareerStart = PersonParser.FindCareerStartInText(text.Except("Karrierestart:"));
            else if (text.StartsWith("Karrierestatus:"))
                Result.StillActive = PersonParser.FindStillActiveInText(text.Except("Karrierestatus:"));
            else if (text.StartsWith("Rasse:"))
                Result.Ethnicity = text.Except("Rasse:");
            else if (text.StartsWith("Haare:"))
                Result.HairColor = text.Except("Haare:");
            else if (text.StartsWith("Augen:"))
                Result.EyeColor = text.Except("Augen:");
            else if (text.StartsWith("Maße:"))
                UpdateMeasurements(text.Except("Maße:"), true);
            else if (text.StartsWith("Körbchengröße:"))
                UpdateMeasurements(text.Except("Körbchengröße:"), true);
            else if (text.StartsWith("Größe:"))
                Result.Height = PersonParser.FindHeightInText(text);
            else if (text.StartsWith("Gewicht:"))
                Result.Weight = PersonParser.FindWeightInText(text);
            else if (text.StartsWith("Piercings:"))
                Result.Piercings = text.Except("Piercings:", "None");
        }
        return Task.CompletedTask;
    }

}