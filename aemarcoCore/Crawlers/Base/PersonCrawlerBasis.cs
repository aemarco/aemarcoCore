using aemarcoCore.Common;
using aemarcoCore.Crawlers.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class PersonCrawlerBasis : CrawlerBasis
    {

        #region fields

        private CancellationToken _cancellationToken;
        protected string _nameToCrawl;

        #endregion

        #region ctor

        internal PersonCrawlerBasis(
            string nameToCrawl,
            CancellationToken cancellationToken)
        {
            _nameToCrawl = nameToCrawl;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region props


        /// <summary>
        /// used for crawl filtering
        /// </summary>
        internal abstract PersonSite PersonSite { get; }
        /// <summary>
        /// higher means priority, lower means not so meaningfull
        /// </summary>
        internal abstract int PersonPriority { get; }

        #endregion

        #region Starting 

        internal bool Start()
        {
            //Crawling
            DoWork();

            if (_cancellationToken.IsCancellationRequested)
            {
                return false;

            }
            return true;
        }

        #endregion

        #region Crawling

        private void DoWork()
        {
            PersonEntry entry = GetPersonEntry();
            OnProgress(100);
            if (entry.IsValid)
            {
                OnEntry(entry);
            }
        }

        internal abstract PersonEntry GetPersonEntry();





        /// <summary>
        /// used to convert "34B-26-35" into "86-66-88"
        /// </summary>
        /// <param name="temp">"34B-26-35"</param>
        /// <param name="isCmAlready">true means skip multiplication with 2.54</param>
        /// <returns>"86-66-88"</returns>
        protected string ConvertMaßeToMetric(string temp, bool isCmAlready = false)
        {
            List<int> entries = new List<int>();

            foreach (var entry in temp.Split('-'))
            {
                var resultString = Regex.Match(entry, @"\d+").Value;
                if (int.TryParse(resultString, out int number))
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
            string result = null;

            if (!string.IsNullOrWhiteSpace(temp) && temp.Contains("-"))
            {
                var first = temp.Split('-')[0];
                var resultString = Regex.Match(first, @"[^\d]+").Value;
                if (!string.IsNullOrWhiteSpace(resultString)) result = resultString;

            }
            return result;
        }


        /// <summary>
        /// used to convert feet and inches to cm
        /// </summary>
        /// <param name="size">5' 5"</param>
        /// <returns>165 as nullable int</returns>
        protected int? ConvertFeetAndInchToCm(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;


            int inches = 0;
            if (str.Contains("'"))
            {
                var feetstring = str.Substring(0, str.IndexOf("'")).Trim();
                if (int.TryParse(feetstring, out int feet)) inches += feet * 12;
                str = str.Substring(str.IndexOf("'") + 1);
            }
            if (str.Contains("\""))
            {
                var inchstring = str.Substring(0, str.IndexOf("\"")).Trim();
                if (int.TryParse(inchstring, out int inch)) inches += inch;
            }

            int cm = (int)(inches * 2.54);
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


            int kilos = 0;

            if (int.TryParse(resultString, out int lbs))
            {
                kilos = (int)(1.0 * lbs / 2.20462);
            }

            return (kilos > 0) ? kilos : default(int?);
        }



        #endregion

        #region Events

        internal event EventHandler<ProgressChangedEventArgs> Progress;
        protected void OnProgress(int prog)
        {
            Progress?.Invoke(this, new ProgressChangedEventArgs(prog, null));
        }

        internal event EventHandler<IPersonEntryEventArgs> Entry;
        private void OnEntry(IPersonEntry entry)
        {
            Entry?.Invoke(this, new IPersonEntryEventArgs { Entry = entry });
        }

        #endregion
    }
}
