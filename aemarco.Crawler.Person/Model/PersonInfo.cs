using System;
using System.Collections.Generic;
using System.Linq;
using aemarco.Crawler.Person.Base;
using aemarco.Crawler.Person.Common;

namespace aemarco.Crawler.Person.Model
{
    public class PersonInfo
    {
        internal PersonInfo(PersonCrawlerBase crawler)
        {
            var info = crawler.GetType().ToCrawlerInfo();
            PersonEntrySource = info.FriendlyName;
            PersonEntryPriority = info.Priority;


            ProfilePictures = new List<ProfilePicture>();
            Aliases = new List<string>();
        }

        internal string PersonEntrySource { get; }
        internal int PersonEntryPriority { get; }


        internal void Merge(PersonInfo info)
        {
            //first degree match
            if (info.FirstName != FirstName || info.LastName != LastName)
            {
                //second degree match
                if (!Aliases.Contains($"{info.FirstName} {info.LastName}") &&
                    !info.Aliases.Contains($"{FirstName} {LastName}"))
                {
                    return;
                }
            }

            ProfilePictures.AddRange(info.ProfilePictures);
            ProfilePictures = ProfilePictures.Distinct().ToList();
            Birthday = Birthday ?? info.Birthday;
            Country = Country ?? info.Country;
            City = City ?? info.City;
            Profession = Profession ?? info.Profession;
            CareerStart = CareerStart ?? info.CareerStart;
            StillActive = StillActive ?? info.StillActive;
            Aliases.AddRange(info.Aliases);
            Aliases = Aliases.Distinct().OrderBy(x => x).ToList();
            Ethnicity = Ethnicity ?? info.Ethnicity;
            HairColor = HairColor ?? info.HairColor;
            EyeColor = EyeColor ?? info.EyeColor;
            Measurements = Measurements ?? info.Measurements;
            CupSize = CupSize ?? info.CupSize;
            Height = Height ?? info.Height;
            Weight = Weight ?? info.Weight;
            Piercings = Piercings ?? info.Piercings;
        }

        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }


        public List<ProfilePicture> ProfilePictures { get; internal set; }
        public DateTime? Birthday { get; internal set; }
        public string Country { get; internal set; }
        public string City { get; internal set; }
        public string Profession { get; internal set; }
        public DateTime? CareerStart { get; internal set; }
        public bool? StillActive { get; internal set; }
        public List<string> Aliases { get; internal set; }
        public string Ethnicity { get; internal set; }
        public string HairColor { get; internal set; }
        public string EyeColor { get; internal set; }
        public string Measurements { get; internal set; }
        public string CupSize { get; internal set; }
        public int? Height { get; internal set; }
        public int? Weight { get; internal set; }
        public string Piercings { get; internal set; }
    }
}
