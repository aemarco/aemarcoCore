using System.Linq;

namespace aemarcoCore.Common
{
    public static class Extensions
    {
        public static bool Supports(this SourceSite site, string category)
        {
            var memInfo = typeof(SourceSite).GetMember(site.ToString());
            var attributes = (SupportedCategoriesAttribute[])memInfo[0].GetCustomAttributes(typeof(SupportedCategoriesAttribute), false);
            return attributes.Any(x => x.Categories.Contains(category));
        }
    }
}
