using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using aemarcoCore.Crawlers.Types;

namespace aemarcoCore.Common
{
    public static class Extensions
    {
        public static bool IsDisabled(this SourceSite site)
        {
            var attr = site.GetAttribute<DisabledAttribute>();
            return attr != null;
        }

        public static bool SupportsAny(this SourceSite site, IEnumerable<string> categories)
        {
            return categories.Any(x => site.Supports(x));
        }


        public static bool Supports(this SourceSite site, string category)
        {
            var attr = site.GetAttribute<SupportedCategoriesAttribute>();
            return attr?.Categories.Contains(category) ?? false;
        }


        public static T GetAttribute<T>(this Enum value)
            where T : Attribute
        {
            var enumMember = value.GetType().GetMember(value.ToString()).First();
            var attributes = enumMember.GetCustomAttributes(typeof(T), false).Cast<T>();
            return attributes.FirstOrDefault();
        }





        //person crawling extensions
        internal static void IncludeProfilePicture(this PersonEntry entry, string url, int suggestedMinAdult = -1, int suggestedMaxAdult = -1)
        {
            entry.ProfilePictures.Add(new ProfilePicture
            {
                Url = url,
                SuggestedMinAdultLevel =  suggestedMinAdult,
                SuggestedMaxAdultLevel = suggestedMaxAdult
            });
        }

        internal static void IncludeStillActive(this PersonEntry entry, string text)
        {
            if (Regex.IsMatch(text, "active", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(text, "present", RegexOptions.IgnoreCase))
                entry.StillActive = true;


            if (Regex.IsMatch(text, "retired", RegexOptions.IgnoreCase) ||
                Regex.Match(text, @"\d+").Groups.Count == 2) // 2013 - 2015
                entry.StillActive = false;
        }




    }
}
