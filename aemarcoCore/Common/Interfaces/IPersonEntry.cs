using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IPersonEntry
    {
        string PersonEntrySource { get; }
        int PersonEntryPriority { get; }

        string FirstName { get; }
        string LastName { get; }
        string PictureUrl { get; }
        int PictureSuggestedAdultLevel { get; }
        DateTime? Geburtstag { get; }
        string Land { get; }
        string Geburtsort { get; }
        string Beruf { get; }
        DateTime? Karrierestart { get; }
        string Karrierestatus { get; }
        List<string> Aliase { get; }
        string Rasse { get; }
        string Haare { get; }
        string Augen { get; }
        string Maße { get; }
        string Körbchengröße { get; }
        int? Größe { get; }
        int? Gewicht { get; }
        string Piercings { get; }

        [JsonIgnore]
        string JSON { get; }
    }
}
