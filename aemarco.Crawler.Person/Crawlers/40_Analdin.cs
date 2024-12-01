namespace aemarco.Crawler.Person.Crawlers;

[Crawler("Analdin", 40)]
internal class Analdin : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://www.analdin.xxx/");

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/models/{name}/"
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://www.analdin.xxx/models/naveen-ora/
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        //Picture
        UpdateProfilePictures(girlPage
            .FindNode(("//div[@class='model-image-holder']/div/noindex/img"))?
            .GetSrc());

        if (girlPage.FindNode("//div[@class='bm-right']") is { } bmRight)
        {
            //Name
            UpdateName(bmRight.FindNode("./div[@class='headline']"));

            //alias
            foreach (var desc in bmRight.FindNodes("./div[@class='desc']"))
            {
                if (desc.FindNode("./strong") is { } label &&
                    label.GetText().StartsWith("Alias") &&
                    desc.FindNode("./span") is { } alSpan)
                {
                    var alText = alSpan.GetText();
                    Result.Aliases.AddRange(alText.SplitList());
                }
            }

        }

        var dataNodes = girlPage.FindNodes("//ul[@class='model-list']/li");
        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();

            if (node.GetText().Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                is not { Length: >= 2 } parts)
            {
                continue;
            }

            var label = parts[0];
            var text = string.Join(":", parts[1..]);
            if (string.IsNullOrWhiteSpace(text))
                continue;



            Action act = label switch
            {
                "Body Art" => () => Result.Piercings = text,
                "Hair Color" => () => Result.HairColor = text,
                "Eye Color" => () => Result.EyeColor = text,
                "Country" => () => UpdateCountry(text),
                "City" => () =>
                {
                    if (text.SplitList().FirstOrDefault() is { } city)
                        Result.City = city;
                }
                ,
                "Height" => () => Result.Height = PersonParser.FindHeightInText(text),
                "Weight" => () => Result.Weight = PersonParser.FindWeightInText(text),
                "Official Site" => () =>
                {
                    if (node.FindNode("./noindex/a")?.GetHref() is { } href)
                        UpdateSocial(href);
                }
                ,
                _ => () => { }
            };
            act();


        }


        return Task.CompletedTask;
    }
}
