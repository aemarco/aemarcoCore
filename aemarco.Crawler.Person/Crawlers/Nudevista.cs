namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Nudevista", 20)]
internal class Nudevista : PersonCrawlerBase
{
    public Nudevista(string nameToCrawl)
        : base(nameToCrawl, new Uri("https://www.nudevista.at"))
    { }


    protected override string GetSiteHref()
    {
        // ?q=Aletta+Ocean&s=s
        var href = $"?q={NameToCrawl.Replace(' ', '+')}&s=s";
        return href;
    }

    protected override Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken)
    {


        //Name
        var nameNode = document.DocumentNode.SelectSingleNode("//td[contains(@valign, 'top') and contains(@colspan ,'2')]");
        AddNameFromInnerText(nameNode);

        //Picture
        var picNode = document.DocumentNode.SelectSingleNode("//img[@class='mthumb']");
        AddProfilePicture(picNode, "src",
            x => new Uri(x.StartsWith("http") ? x : $"https:{x}"));


        //Data
        var nodeWithData = document.DocumentNode.SelectSingleNode("//div[@id='params_scroll']");
        if (nodeWithData is null)
            return Task.CompletedTask;

        foreach (var node in nodeWithData.ChildNodes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeText = GetInnerText(node);

            if (nodeText.StartsWith("Geschlecht:"))
                UpdateGenderFromText(nodeText); // female male tranny
            else if (nodeText.StartsWith("Geburtstag:"))
                Result.Birthday = FindBirthdayInText(nodeText);
            else if (nodeText.StartsWith("Land:"))
                Result.Country = GetInnerText(node, removals: "Land:");
            else if (nodeText.StartsWith("Geburtsort:"))
                Result.City = GetInnerText(node, removals: "Geburtsort:");
            else if (nodeText.StartsWith("Beruf:"))
                Result.Profession = GetInnerText(node, removals: "Beruf:");
            else if (nodeText.StartsWith("Karrierestart:"))
                Result.CareerStart = FindCareerStartInText(nodeText);
            else if (nodeText.StartsWith("Karrierestatus:"))
                Result.StillActive = FindStillActiveInText(node.InnerText);
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

                    if (al.Length > 3 && al.Contains(" "))
                    {
                        Result.Aliases.Add(al);
                    }

                }
            }
            else if (nodeText.StartsWith("Rasse:"))
                Result.Ethnicity = GetInnerText(node, removals: "Rasse:");
            else if (nodeText.StartsWith("Haare:"))
                Result.HairColor = GetInnerText(node, removals: "Haare:");
            else if (nodeText.StartsWith("Augen:"))
                Result.EyeColor = GetInnerText(node, removals: "Augen:");
            else if (nodeText.StartsWith("Maße:"))
                UpdateFromMeasurementsText(nodeText, true);
            else if (nodeText.StartsWith("Körbchengröße:"))
                UpdateFromCupText(GetInnerText(node, removals: "Körbchengröße:"));
            else if (nodeText.StartsWith("Größe:"))
                Result.Height = FindHeightInText(nodeText);
            else if (nodeText.StartsWith("Gewicht:"))
                Result.Weight = FindWeightInText(nodeText);
            else if (nodeText.StartsWith("Piercings:"))
                Result.Piercings = GetInnerText(node, removals: new[] { "Piercings:", "None" });


        }
        return Task.CompletedTask;

    }



}