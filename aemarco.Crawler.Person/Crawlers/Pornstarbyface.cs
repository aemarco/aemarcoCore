namespace aemarco.Crawler.Person.Crawlers;

[PersonCrawler("Pornstarbyface", 0)]
internal class Pornstarbyface : PersonCrawlerBase
{
    public Pornstarbyface(string nameToCrawl)
        : base(nameToCrawl)
    { }

    private readonly Uri _uri = new("https://pornstarbyface.com/");

    internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
    {
        var result = new PersonInfo(this);

        // /models/foxy-di/biography
        var href = $"/girls/{NameToCrawl.Replace(' ', '-')}";
        var target = new Uri(_uri, href);
        var document = HtmlHelper.GetHtmlDocument(target);


        var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='star-info']/h5");
        var nodeWithPicture = document.DocumentNode.SelectSingleNode("//div[@class='col-lg-3 profile-image']/img[@class='img-responsive']");
        var nodesWithData = document.DocumentNode.SelectNodes("//div[@class='star-info']/div/b");

        //Name
        if (nodeWithName != null)
        {
            var n = GetInnerText(nodeWithName);
            n = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(n.ToLower());
            if (n.Contains(" "))
            {
                result.FirstName = n[..n.IndexOf(' ')];
                result.LastName = n[(n.IndexOf(' ') + 1)..];
            }
        }

        //Pictures
        if (nodeWithPicture?.Attributes["src"]?.Value is { } imageRef)
        {
            var uri = new Uri(_uri, WebUtility.HtmlDecode(imageRef));
            result.ProfilePictures.Add(new ProfilePicture(uri.AbsoluteUri));
        }



        //Data
        if (nodesWithData == null)
            return Task.FromResult(result);


        HtmlNode? GetValueNode(HtmlNode node)
        {
            return node.ParentNode.NextSibling;
        }

        foreach (var node in nodesWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (node.InnerText.StartsWith("Aliases") && GetValueNode(node) is { } alsNode)
            {
                var als = alsNode.InnerText.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));
                result.Aliases.AddRange(als);
            }
            else if (node.InnerText.StartsWith("Country") && GetValueNode(node) is { } countryNode)
            {
                result.Country = GetInnerText(countryNode);
            }
            else if (node.InnerText.StartsWith("State") && GetValueNode(node) is { } cityNode)
            {
                result.City = GetInnerText(cityNode);
            }
            else if (node.InnerText.StartsWith("Birthday") && GetValueNode(node) is { } bornNode)
            {
                var str = GetInnerText(bornNode);


                var matches = Regex.Matches(str, @"\d+");
                var mNames = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.ToList();

                if (matches.Count == 2 &&
                    int.TryParse(matches[0].Value, out var day) &&
                    int.TryParse(matches[1].Value, out var year) &&
                    mNames.Any(x => !string.IsNullOrWhiteSpace(x) && Regex.IsMatch(str, x)))
                {
                    var monthName = mNames.First(x => Regex.IsMatch(str, x));
                    var index = mNames.IndexOf(monthName);
                    var month = index + 1;
                    result.Birthday = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

                }
            }
            else if (node.InnerText.StartsWith("Ethnicity") && GetValueNode(node) is { } ethNode)
            {
                result.Ethnicity = GetInnerText(ethNode);
            }

            else if (node.InnerText.StartsWith("Eye") && GetValueNode(node) is { } eyeNode)
            {
                result.EyeColor = GetInnerText(eyeNode);
            }
            else if (node.InnerText.StartsWith("Hair") && GetValueNode(node) is { } hairNode)
            {
                result.HairColor = GetInnerText(hairNode);
            }
            else if (node.InnerText.StartsWith("Height") && GetValueNode(node) is { } heightNode)
            {
                result.Height = ConvertFeetAndInchToCm(GetInnerText(heightNode));
            }
            else if (node.InnerText.StartsWith("Weight") && GetValueNode(node) is { } weightNode)
            {
                result.Weight = ConvertLibsToKg(GetInnerText(weightNode));
            }
            else if (node.InnerText.StartsWith("Cup") && GetValueNode(node) is { } cupNode)
            {
                result.CupSize = GetInnerText(cupNode);
            }
            else if (node.InnerText.StartsWith("Measurements") && GetValueNode(node) is { } measureNode)
            {
                result.Measurements = ConvertMeasurementsToMetric(GetInnerText(measureNode));
            }
        }

        return Task.FromResult(result);

    }



}