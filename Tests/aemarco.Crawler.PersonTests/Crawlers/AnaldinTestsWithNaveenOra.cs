namespace aemarco.Crawler.PersonTests.Crawlers;
internal class AnaldinTestsWithNaveenOra : PersonCrawlerTestsBase<Analdin>
{

    //https://www.analdin.xxx/models/naveen-ora/
    public AnaldinTestsWithNaveenOra()
        : base("Naveen", "Ora")
    {
        //first and last name expected automatically


        ExpectedProfilePictures =
        [
            "https://i.analdin.com/contents/models/98/s2_naveen_ora02.jpg"
        ];

        ExpectedAliases =
        [
            "Haven Aspen",
            "Aspen Ora"
        ];
        ExpectedPiercings = "multiple piercings in nipples, navel, lower back, lip";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Brown";
        ExpectedCountry = "United States";
        ExpectedCity = "Beverly Hills";
        ExpectedHeight = 175;
        ExpectedWeight = 58;
        ExpectedSocialLinks =
        [
            new SocialLink(
                SocialLinkKind.Twitter,
                "https://twitter.com/Aspen_Ora")
        ];


    }
}
