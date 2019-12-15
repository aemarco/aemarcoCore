﻿using aemarcoCore.Crawlers.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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












    }
}
