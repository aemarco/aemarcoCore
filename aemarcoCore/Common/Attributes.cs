using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCore.Common
{
    public class SupportedCategoriesAttribute : Attribute
    {
        public List<string> Categories { get; }

        public SupportedCategoriesAttribute(params Category[] categories)
        {
            Categories = categories.Select(x => x.ToString()).ToList();
        }
    }
}
