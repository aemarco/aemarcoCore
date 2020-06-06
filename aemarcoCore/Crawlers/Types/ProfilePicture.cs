using aemarcoCore.Common;

namespace aemarcoCore.Crawlers.Types
{
    internal class ProfilePicture : IProfilePicture
    {
        public string Url { get; set; }
        public int SuggestedMinAdultLevel { get; set; }
        public int SuggestedMaxAdultLevel { get; set; }
    }
}
