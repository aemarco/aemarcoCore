
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace aemarcoCore.Common
{
    public interface IPersonCrawlerResult
    {
        List<IPersonEntry> Entries { get; }
        Exception Exception { get; set; }
        bool HasBeenAborted { get; set; }
        string Json { get; }
        string ResultName { get; set; }
    }

    public interface IPersonEntry
    {
        string PersonEntrySource { get; }
        int PersonEntryPriority { get; }

        string FirstName { get; }
        string LastName { get; }

        List<IProfilePicture> ProfilePictures { get; }


        DateTime? Geburtstag { get; }
        string Land { get; }
        string Geburtsort { get; }
        string Beruf { get; }
        DateTime? Karrierestart { get; }
        bool? StillActive { get;  }
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
        string Json { get; }
    }

    public interface IProfilePicture
    {
        string Url { get; }
        int SuggestedMinAdultLevel { get; }
        int SuggestedMaxAdultLevel { get; }
    }



}