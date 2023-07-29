namespace aemarco.Crawler.Person.Crawlers;

[Crawler("TheLordOfPorn", 11)]
internal class TheLordOfPorn : PersonCrawlerBase
{

    private readonly Uri _uri = new("https://thelordofporn.com");


    //z.B. "https://thelordofporn.com/pornstars/dakota-tyler"
    protected override PageUri GetGirlUri(string nameToCrawl) =>
        new PageUri(_uri).WithHref($"/pornstars/{nameToCrawl.Replace(' ', '-').ToLower()}");

    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
    {
        //Name
        UpdateName(girlPage.FindNode("//h1[@class='entry-title']"));

        //Pics
        var contentNode = girlPage.FindNode("//div[@class='entry-content']");
        contentNode?  //click able big once
            .FindNodes("./a")
            .Select(x => x.GetHref())
            .OfType<PageUri>()
            .ToList()
            .ForEach(x => UpdateProfilePictures(x));
        contentNode? //embedded small once
            .FindNodes("./descendant::img")
            .Where(x => x.Node.ParentNode.Name != "a")
            .Select(x => x.GetAttributeRef("data-lazy-src"))
            .OfType<PageUri>()
            .ToList()
            .ForEach(x => UpdateProfilePictures(x));

        //Rating
        Result.Rating = PersonParser.FindRatingInText(girlPage
            .FindNode("//span[@class='stars-rating']")?
            .GetAttribute("data-vote"));

        //Data
        var dataNodes = girlPage.FindNodes("//table[@class='spectable']/tr");
        Result.Gender = Gender.Female; //always female on this site
        foreach (var dataNode in dataNodes)
        {
            var desc = dataNode.FindNode("./td")?.GetText();
            var textNode = dataNode.FindNode("./td[@class='sval']");
            var text = textNode?.GetText();


            Action act = desc switch
            {
                "Name" => () => Result.Aliases.AddRange(text.SplitList()),
                "Born" => () => Result.Birthday = text.ToDateOnly(),
                "Height" => () => Result.Height = PersonParser.FindHeightInText(text),
                "Weight" => () => Result.Weight = PersonParser.FindWeightInText(text),
                "Measurements" => () => UpdateMeasurements(text, true),
                "Official Website" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref(), SocialLinkKind.Official),
                "Twitter" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
                "Facebook" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
                "Instagram" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
                _ => () => { }
            };
            act();
        }

        return Task.CompletedTask;
    }
}
