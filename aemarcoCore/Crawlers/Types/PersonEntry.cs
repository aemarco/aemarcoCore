using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class PersonEntry : IPersonEntry
    {
        private readonly string _nameToCrawl;
        public PersonEntry(string nameToCrawl)
        {
            _nameToCrawl = nameToCrawl;

            Aliase = new List<string>();
        }

        public string PersonEntrySource { get; set; }
        public int PersonEntryPriority { get; set; }

        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string PictureUrl { get; internal set; }
        public DateTime? Geburtstag { get; internal set; }
        public string Land { get; internal set; }
        public string Geburtsort { get; internal set; }
        public string Beruf { get; internal set; }
        public DateTime? Karrierestart { get; internal set; }
        public string Karrierestatus { get; internal set; }
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
        internal bool IsValid
        {
            get
            {

                if (
                    String.IsNullOrEmpty(FirstName) || //Entry muss FirstName haben
                    String.IsNullOrEmpty(LastName) || //Entry muss LastName haben
                    String.IsNullOrEmpty(PictureUrl) //Entry muss PictureUrl haben
                    )

                {
                    return false;
                }
                return true;
            }
        }





    }
}
