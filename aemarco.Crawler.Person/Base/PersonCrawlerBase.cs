using aemarcoCommons.PersonCrawler.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.PersonCrawler.Base
{
    internal abstract class PersonCrawlerBase
    {
        internal PersonCrawlerBase(string nameToCrawl)
        {
            NameToCrawl = nameToCrawl;
        }
        internal string NameToCrawl { get; }


        internal abstract Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken);



        protected bool? IsStillActive(string text)
        {
            if (Regex.IsMatch(text, "active", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(text, "present", RegexOptions.IgnoreCase))
                return true;


            if (Regex.IsMatch(text, "retired", RegexOptions.IgnoreCase) ||
                Regex.Match(text, @"\d+").Groups.Count == 2) // 2013 - 2015
                return false;

            return null;
        }



        /// <summary>
        /// used to convert "34B-26-35" into "86-66-88"
        /// </summary>
        /// <param name="temp">"34B-26-35"</param>
        /// <param name="isCmAlready">true means skip multiplication with 2.54</param>
        /// <returns>"86-66-88"</returns>
        protected string ConvertMaßeToMetric(string temp, bool isCmAlready = false)
        {
            var entries = new List<int>();

            foreach (var entry in temp.Split('-'))
            {
                var resultString = Regex.Match(entry, @"\d+").Value;
                if (int.TryParse(resultString, out var number))
                {
                    if (!isCmAlready) number = (int)(number * 2.54);
                    entries.Add(number);
                }
            }

            if (entries.Count == 3)
                return string.Join("-", entries);
            else
                return null;
        }


        /// <summary>
        /// used to extract cupsize "B" from "34B-26-35"
        /// </summary>
        /// <param name="temp">"34B-26-35"</param>
        /// <returns>"B"</returns>
        protected string ConvertMaßeToCupSize(string temp)
        {
            if (string.IsNullOrWhiteSpace(temp)) return null;
            var first = temp.Split('-')[0];
            var resultString = Regex.Match(first, @"[^\d]+").Value;
            return resultString;

        }


        /// <summary>
        /// used to convert feet and inches to cm
        /// </summary>
        /// <param name="str">5' 5"</param>
        /// <returns>165 as nullable int</returns>
        protected int? ConvertFeetAndInchToCm(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;


            var inches = 0;
            if (str.Contains("'"))
            {
                var feetstring = str.Substring(0, str.IndexOf("'")).Trim();
                if (int.TryParse(feetstring, out var feet)) inches += feet * 12;
                str = str.Substring(str.IndexOf("'") + 1);
            }
            if (str.Contains("\""))
            {
                var inchstring = str.Substring(0, str.IndexOf("\"")).Trim();
                if (int.TryParse(inchstring, out var inch)) inches += inch;
            }

            var cm = (int)(inches * 2.54);
            return (cm > 0) ? cm : default(int?);
        }


        /// <summary>
        /// used to convert libs to kg
        /// </summary>
        /// <param name="str">99 lbs</param>
        /// <returns>44 as nullable int</returns>
        protected int? ConvertLibsToKg(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            var resultString = Regex.Match(str, @"\d+").Value;
            if (string.IsNullOrWhiteSpace(resultString)) return null;


            var kilos = 0;

            if (int.TryParse(resultString, out var lbs))
            {
                kilos = (int)(1.0 * lbs / 2.20462);
            }

            return (kilos > 0) ? kilos : default(int?);
        }



    }
}
