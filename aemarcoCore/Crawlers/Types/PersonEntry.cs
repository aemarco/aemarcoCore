using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class PersonEntry : IPersonEntry
    {
       
        public PersonEntry(PersonCrawlerBase crawler)
        {
            PersonEntrySource = crawler.PersonSite.ToString();
            PersonEntryPriority = crawler.PersonPriority;
            Aliase = new List<string>();
            ProfilePictures = new List<IProfilePicture>();
        }

        public string PersonEntrySource { get; }
        public int PersonEntryPriority { get; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public List<IProfilePicture> ProfilePictures { get; }
        public DateTime? Geburtstag { get; internal set; }
        public string Land { get; internal set; }
        public string Geburtsort { get; internal set; }
        public string Beruf { get; internal set; }
        public DateTime? Karrierestart { get; internal set; }
        public bool? StillActive { get; internal set; }
        public List<string> Aliase { get; internal set; }
        public string Rasse { get; internal set; }
        public string Haare { get; internal set; }
        public string Augen { get; internal set; }
        public string Maße { get; internal set; }
        public string Körbchengröße { get; internal set; }
        public int? Größe { get; internal set; }
        public int? Gewicht { get; internal set; }
        public string Piercings { get; internal set; }


        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }




    
        [JsonIgnore]
        internal bool IsValid =>
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName);
    }
}
