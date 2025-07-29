//using System.Net;

//namespace aemarco.Crawler.Person.Crawlers;


//[Crawler("TheLordOfPorn", 11)]
//internal class TheLordOfPorn : SiteCrawlerBase
//{

//    private readonly Uri _uri = new("https://thelordofporn.com");
//    public TheLordOfPorn(
//        ICountryService countryService,
//        ILogger<Freeones> logger)
//        : base(countryService, logger)
//    {

//    }


//    protected override PageUri GetGirlUri(string name)
//    {
//        name = name
//            .Replace(' ', '-')
//            .ToLower();
//        var result = new PageUri(_uri)
//            .WithHref($"/pornstars/{name}");

//        //https://thelordofporn.com/pornstars/dakota-tyler


//        result = new PageUri(new Uri($"https://archive.ph/submit/?url={WebUtility.UrlEncode(result.Uri.AbsoluteUri)}"));


//        //https://archive.ph/submit/?url=https%3A%2F%2Fthelordofporn.com%2Fpornstars%2Fdakota-tyler


//        return result;
//    }

//    protected override Task HandlePersonEntry(PageDocument girlPage, CancellationToken token)
//    {
//        //Name
//        var nameNode = girlPage.FindNode("//h1[@class='entry-title']");
//        UpdateName(nameNode);

//        //Pics
//        var contentNode = girlPage.FindNode("//div[@class='entry-content']");

//        //big once are in a wrong ratio :(

//        //contentNode?  //click able big once
//        //    .FindNodes("./a")
//        //    .Select(x => x.GetHref())
//        //    .OfType<PageUri>()
//        //    .ToList()
//        //    .ForEach(x => UpdateProfilePictures(x));
//        //contentNode?  //non click able big once
//        //    .FindNodes("./div/img")
//        //    .Select(x => x.GetAttributeRef("data-lazy-src"))
//        //    .OfType<PageUri>()
//        //    .ToList()
//        //    .ForEach(x => UpdateProfilePictures(x));

//        contentNode?  //embedded click able once
//            .FindNodes("./p/a")
//            .Select(x => x.GetHref())
//            .OfType<PageUri>()
//            .ToList()
//            .ForEach(x => UpdateProfilePictures(x));
//        contentNode? //embedded non click able once
//            .FindNodes("./descendant::img")
//            .Where(x => x.Parent().Node.Name != "a")
//            .Where(x => x.Parent().Node.Name != "div")
//            .Select(x => x.GetAttributeRef("data-lazy-src"))
//            .OfType<PageUri>()
//            .ToList()
//            .ForEach(x => UpdateProfilePictures(x));


//        //Rating
//        var ratingNode = girlPage.FindNode("//label[@class='rating__label']");
//        Result.Rating = PersonParser.FindRatingInText(ratingNode?.GetText(), true);

//        //Data
//        var dataNodes = girlPage.FindNodes("//ul[@class='specifications__list lines-list']/li");
//        Result.Gender = Gender.Female; //always female on this site
//        foreach (var dataNode in dataNodes)
//        {
//            var desc = dataNode.FindNode("./b")?.GetText();
//            var textNode = dataNode.FindNode("./span");
//            var text = textNode?.GetText();


//            Action act = desc switch
//            {
//                "Name" => () => Result.Aliases.AddRange(text.SplitList()),
//                "Born" => () => Result.Birthday = text.ToDateOnly(),
//                "Height" => () => Result.Height = PersonParser.FindHeightInText(text),
//                "Weight" => () => Result.Weight = PersonParser.FindWeightInText(text),
//                "Measurements" => () => UpdateMeasurements(text, true),
//                //"Official Website" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref(), SocialLinkKind.Official),
//                //"Twitter" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
//                //"Facebook" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
//                //"Instagram" => () => UpdateSocial(textNode?.FindNode("./a")?.GetHref()),
//                _ => () => { }
//            };
//            act();
//        }

//        var socialNodes = girlPage.FindNodes("//ul[@class='social']/li/a");
//        foreach (var socialNode in socialNodes)
//        {
//            var parentClass = socialNode.Parent().GetAttribute("class");
//            var socialKind = parentClass switch
//            {
//                "social__item social__item--main" => SocialLinkKind.Official,
//                _ => SocialLinkKind.Unknown //others detected by url
//            };
//            UpdateSocial(socialNode.GetHref(), socialKind);
//        }







//        return Task.CompletedTask;
//    }
//}
