namespace aemarco.Crawler.Person.Crawlers;

[Crawler("XxxBios", 13)]
internal class XxxBios : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://xxxbios.com");

    protected override PageUri GetGirlUri(string name)
    {
        var href = $"/{name}-biography"
            .Replace(' ', '-');
        var result = new PageUri(_uri)
            .WithHref(href);

        //https://xxxbios.com/hannah-hays-biography/
        return result;
    }

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        var dataNodes = girlPage.FindNodes("//div[@class='clearfix entry-content']/p");

        foreach (var node in dataNodes)
        {
            token.ThrowIfCancellationRequested();


            if (node.FindNode("./a/img") is { } picNode)
            {
                UpdateProfilePictures(picNode.GetAttributeRef("data-src"));
            }
            else if (node.FindNode("./a") is { } linkNode)
            {
                UpdateWellKnownSocial(linkNode.GetHref());
            }

            if (node.FindNode("./strong") is { } labelNode)
            {
                var label = labelNode.GetText();
                var text = node.SelectNode(x => x.LastChild).GetText();
                Action act = label switch
                {
                    "Name :" => () => UpdateName(node.SelectNode(x => x.LastChild)),
                    "Hometown :" => () =>
                    {
                        Result.City = text;
                        UpdateCountry(text.TextInParentheses());
                    }
                    ,
                    "Measurements :" => () => UpdateMeasurements(text, true),
                    "Height :" => () => Result.Height = PersonParser.FindHeightInText(text),
                    "Hair Colour :" => () => Result.HairColor = text,
                    "Eye Colour :" => () => Result.EyeColor = text,
                    "Piercings :" => () => Result.Piercings = text,
                    "Years Active :" => () =>
                    {
                        Result.CareerStart = PersonParser.FindCareerStartInText(text);
                        Result.StillActive = PersonParser.FindStillActiveInText(text);
                    }
                    ,
                    _ => () => { }
                };
                act();
            }

        }


        return Task.CompletedTask;
    }
}
