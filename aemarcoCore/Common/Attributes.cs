using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCore.Common
{






    public sealed class SupportedCategoriesAttribute : Attribute
    {
        public List<string> Categories { get; }

        public SupportedCategoriesAttribute(params Category[] categories)
        {
            Categories = categories.Select(x => x.ToString()).ToList();
        }
    }









    public sealed class DisabledAttribute : Attribute
    {
        public string Message { get; }
        public DisabledAttribute(string message = "")
        {
            Message = message;
        }
    }
}
