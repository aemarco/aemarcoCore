namespace aemarco.Crawler.PersonTests.Crawlers_PersonInfo;

internal class BabepediaTestsWithFoxiDi : PersonInfoTestsBase<Babepedia>

{

    //https://www.babepedia.com/babe/Foxy_Di
    public BabepediaTestsWithFoxiDi()
        : base("Foxi", "Di")
    {
        ExpectedProfilePictures.AddRange([
            "https://www.babepedia.com/pics/Foxy%20Di.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di2.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di3.jpg",
            "https://www.babepedia.com/pics/Foxy%20Di4.jpg"
        ]);

        ExpectedSocialLinks =
        [
            new SocialLink(SocialLinkKind.Vk, "https://vk.com/katerinochka111"),
            new SocialLink(SocialLinkKind.Twitter, "https://x.com/foxydi_"),
            new SocialLink(SocialLinkKind.Instagram, "https://instagram.com/foxy__di"),
            new SocialLink(SocialLinkKind.Facebook, "https://www.facebook.com/FoxyDiModelOFFICIAL"),
            new SocialLink(SocialLinkKind.TikTok, "https://www.tiktok.com/@foxydibabe")
        ];
        ExpectedAliases.AddRange([
            "Angel",
            "Angel C",
            "Ekaterina D",
            "Ekaterina Ivanova",
            "Foxi Di",
            "Foxy B",
            "Foxy Di",
            "Inga",
            "Inna",
            "Kat",
            "Kate",
            "Katoa",
            "Katya Ivanova",
            "Kleine Punci",
            "Marisha",
            "Medina U",
            "Nensi",
            "Nensi B"
        ]);
        ExpectedRating = true;
        ExpectedBirthday = new DateOnly(1994, 9, 14);
        ExpectedCountry = "Russia";
        ExpectedCity = "St. Petersburg";
        ExpectedEthnicity = "Caucasian";
        ExpectedProfession = "Adult Model (Former), Model, Porn Star (Former)";
        ExpectedHairColor = "Brown";
        ExpectedEyeColor = "Green";
        ExpectedHeight = 157;
        ExpectedWeight = 45;
        ExpectedMeasurementDetails = "86B-60-86";
        ExpectedCareerStart = new DateOnly(2013, 1, 1);
        ExpectedStillActive = false;
        ExpectedPiercings = "Navel";
    }
}