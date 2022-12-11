namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("IStripper", 30)]
internal class Stripper : PersonCrawlerBase
{

    public Stripper(string nameToCrawl)
        : base(nameToCrawl)
    { }

    private readonly Uri _uri = new("https://www.istripper.com");


    internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
    {
        var result = new PersonInfo(this);

        var href = $"de/models/{NameToCrawl.Replace(' ', '-')}";
        var target = new Uri(_uri, href);
        var document = HtmlHelper.GetHtmlDocument(target);
        var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='trigger']/div/h1");
        var nodeWithPicture = document.DocumentNode.SelectSingleNode("//div[@class='container']/img");
        var nodeWithData = document.DocumentNode.SelectSingleNode("//ul[@class='info2']");

        //Name
        if (nodeWithName != null)
        {
            var n = nodeWithName.InnerText.Trim();
            n = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(n.ToLower());
            if (n.Contains(" "))
            {
                result.FirstName = n[..n.IndexOf(' ')];
                result.LastName = n[(n.IndexOf(' ') + 1)..];
            }
        }

        //Picture
        if (nodeWithPicture?.Attributes["src"]?.Value is { } picRef)
        {
            result.ProfilePictures.Add(new ProfilePicture(picRef, 35, 39));
        }

        //Data
        if (nodeWithData == null)
            return Task.FromResult(result);


        foreach (var node in nodeWithData.ChildNodes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(node.InnerText))
            {
                continue;
            }


            //Land
            if (node.InnerText.Contains("Land:"))
            {
                result.Country = node.InnerText.Replace("Land:", string.Empty).Trim();
            }
            else if (node.InnerText.Contains("Stadt:"))
            {
                result.City = node.InnerText.Replace("Stadt:", string.Empty).Trim();
            }
            else if (node.InnerText.Contains("Maße:"))
            {
                result.Measurements = node.InnerText.Replace("Maße:", string.Empty)
                    .Replace(" / ", "-")
                    .Trim();
            }
            else if (node.InnerText.Contains("Größe"))
            {
                try
                {
                    var str = node.InnerText.Replace("Größe", string.Empty).Trim();
                    str = str[..(str.IndexOf("cm", StringComparison.Ordinal) - 1)].Trim();
                    result.Height = Convert.ToInt32(str);
                }
                catch
                {
                    // ignored
                }
            }
            else if (node.InnerText.Contains("Gewicht:"))
            {
                try
                {
                    var str = node.InnerText.Replace("Gewicht:", string.Empty).Trim();
                    str = str[..(str.IndexOf("kg", StringComparison.Ordinal) - 1)].Trim();
                    result.Weight = Convert.ToInt32(str);
                }
                catch
                {
                    // ignored
                }
            }

        }

        return Task.FromResult(result);

    }


}