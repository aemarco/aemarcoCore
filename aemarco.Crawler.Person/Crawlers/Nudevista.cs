namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Nudevista", 30)]
internal class Nudevista : PersonCrawlerBase
{

    public Nudevista()
        : base(new Uri("https://www.nudevista.at"))
    { }

    protected override string GetSiteHref(string nameToCrawl)
    {
        // ?q=Aletta+Ocean&s=s
        var href = $"?q={nameToCrawl.Replace(' ', '+')}&s=s";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {


        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//td[contains(@valign, 'top') and contains(@colspan ,'2')]");
        UpdateName(nameNode);

        //Picture
        var picNode = document.DocumentNode.SelectSingleNode("//img[@class='mthumb']");
        if (picNode?.Attributes["src"]?.Value is { } imageRef)
        {
            var url = imageRef.StartsWith("http")
                ? imageRef
                : $"https:{imageRef}";
            UpdateProfilePictures(new Uri(url));
        }



        //Data
        var nodeWithData = document.DocumentNode.SelectSingleNode("//div[@id='params_scroll']");
        if (nodeWithData is null)
            return Task.CompletedTask;

        foreach (var node in nodeWithData.ChildNodes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeText = node.TextWithout();

            if (nodeText.StartsWith("Geschlecht:"))
                Result.Gender = DataParser.FindGenderInText(nodeText); // female male tranny
            else if (nodeText.StartsWith("Geburtstag:"))
                Result.Birthday = DataParser.FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Land:"))
                Result.Country = node.TextWithout("Land:");
            else if (nodeText.StartsWith("Geburtsort:"))
                Result.City = node.TextWithout("Geburtsort:");
            else if (nodeText.StartsWith("Beruf:"))
                Result.Profession = node.TextWithout("Beruf:");
            else if (nodeText.StartsWith("Karrierestart:"))
                Result.CareerStart = DataParser.FindCareerStartInText(nodeText);
            else if (nodeText.StartsWith("Karrierestatus:"))
                Result.StillActive = DataParser.FindStillActiveInText(node.InnerText);
            else if (nodeText.StartsWith("Auch Bekannt Als"))
            {
                //Auch bekannt als Becky Lesabre, Beth Porter.
                var aliasString = node.InnerText.Replace("Auch bekannt als", string.Empty);
                aliasString = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(aliasString.ToLower());
                // Becky Lesabre, Beth Porter.
                if (aliasString.EndsWith("."))
                    aliasString = aliasString.Remove(aliasString.Length - 1);
                // Becky Lesabre, Beth Porter
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
            else if (nodeText.StartsWith("Rasse:"))
                Result.Ethnicity = node.TextWithout("Rasse:");
            else if (nodeText.StartsWith("Haare:"))
                Result.HairColor = node.TextWithout("Haare:");
            else if (nodeText.StartsWith("Augen:"))
                Result.EyeColor = node.TextWithout("Augen:");
            else if (nodeText.StartsWith("Maße:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Körbchengröße:"))
                UpdateMeasurements(nodeText, true);
            else if (nodeText.StartsWith("Größe:"))
                Result.Height = DataParser.FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Gewicht:"))
                Result.Weight = DataParser.FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Piercings:"))
                Result.Piercings = node.TextWithout("Piercings:", "None");
        }
        return Task.CompletedTask;
    }

}