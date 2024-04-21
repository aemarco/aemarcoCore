//namespace aemarco.Crawler.Person.Crawlers;

//[Crawler("Pornsites", 40)]
//internal class Pornsites : PersonCrawlerBase
//{

//    private readonly Uri _uri = new("https://pornsites.xxx");


//    //z.B. "https://pornsites.xxx/pornstars/Aletta-Ocean"
//    protected override PageUri GetGirlUri(string firstName, string lastName) =>
//        new PageUri(_uri).WithHref($"/pornstars/{firstName.Replace(' ', '-')}-{lastName.Replace(' ', '-')}");
//    protected override Task HandleGirlPage(PageDocument girlPage, CancellationToken token)
//    {

//        //Name
//        UpdateName(girlPage.FindNode("//div[@id='main']/header/h1"));

//        //Pics
//        UpdateProfilePictures(girlPage
//            .FindNode("//div[@class='pornstar-box-con-big']/div[@class='pornstar-box']/picture/img")?
//            .GetSrc());

//        //Data
//        var dataNodes = girlPage.FindNodes("//table[@class='styled']/tr");
//        foreach (var node in dataNodes)
//        {
//            token.ThrowIfCancellationRequested();

//            var text = node.GetText().TitleCase();
//            if (text.StartsWith("Age:"))
//                Result.Birthday = text.Except("Age:").ToDateOnly();
//            else if (text.StartsWith("Gender:"))
//                Result.Gender = PersonParser.FindGenderInText(text); //female male trans
//            else if (text.StartsWith("Hair Color:"))
//                Result.HairColor = text.Except("Hair Color:");
//            else if (text.StartsWith("Eye Color:"))
//                Result.EyeColor = text.Except("Eye Color:");
//            else if (text.StartsWith("Cupsize:"))
//                UpdateMeasurements(text.Except("Cupsize:"), true);
//            else if (text.StartsWith("Weight:"))
//                Result.Weight = PersonParser.FindWeightInText(text);
//            else if (text.StartsWith("Height:"))
//                Result.Height = PersonParser.FindHeightInText(text);
//            else if (text.StartsWith("Country:"))
//                Result.Country = text.Except("Country:");
//            else if (text.StartsWith("Ethnicity:"))
//                Result.Ethnicity = text.Except("Ethnicity:");
//        }
//        return Task.CompletedTask;

//    }

//}