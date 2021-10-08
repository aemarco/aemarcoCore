namespace aemarcoCommons.PersonCrawler.Model
{
    public class ProfilePicture
    {
        public static ProfilePicture FromUrl(string url, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
        {
            var result = new ProfilePicture
            {
                Url = url,
                SuggestedMinAdultLevel = suggestedMinAdultLevel,
                SuggestedMaxAdultLevel = suggestedMaxAdultLevel
            };
            return result;
        }

        public string Url { get; set; }
        public int? SuggestedMinAdultLevel { get; set; }
        public int? SuggestedMaxAdultLevel { get; set; }
    }
}
