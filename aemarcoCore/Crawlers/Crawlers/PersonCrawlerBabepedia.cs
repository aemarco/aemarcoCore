using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerBabepedia : PersonCrawlerBase
    {
        public PersonCrawlerBabepedia(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        { }

        private readonly Uri _uri = new Uri("https://www.babepedia.com");
        internal override PersonSite PersonSite => PersonSite.Babepedia;
        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            // /models/foxy-di/biography
            var href = $"/babe/{NameToCrawl.Replace(' ', '_')}";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@id='bioarea']/h1");
            var nodeWithPicture = document.DocumentNode.SelectSingleNode("//div[@id='profimg']/a[@class='img']");
            var nodeWithOtherPictures = document.DocumentNode.SelectNodes("//div[@id='profselect']/div[@class='prof']/a[@class='img']");
            var nodeWithAlias = document.DocumentNode.SelectSingleNode("//div[@id='bioarea']/h2");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//div[@id='bioarea']/ul");

            //Name
            if (nodeWithName != null)
            {
                var n = nodeWithName.InnerText.Trim();
                n = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(n.ToLower());
                if (n.Contains(" "))
                {
                    result.FirstName = n.Substring(0, n.IndexOf(' '));
                    result.LastName = n.Substring(n.IndexOf(' ') + 1);
                }
            }


            //Pictures
            if (nodeWithPicture?.Attributes["href"] != null)
            {
                var uri = new Uri(_uri, nodeWithPicture.Attributes["href"].Value);
                result.IncludeProfilePicture(uri.AbsoluteUri);
            }

            if (nodeWithOtherPictures != null)
            {
                foreach (var node in nodeWithOtherPictures.Where(x => x.Attributes["href"] != null))
                {
                    var uri = new Uri(_uri, node.Attributes["href"].Value);
                    result.IncludeProfilePicture(uri.AbsoluteUri);
                }
            }


            //Alias
            if (nodeWithAlias != null)
            {
                var str = nodeWithAlias.InnerText?.Replace("aka ", string.Empty) ?? string.Empty;
                var als = str.Split('/')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));
                result.Aliase.AddRange(als);
            }


            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    //skipping
                    if (node.Name != "li") continue;
                    
                   
                    if (node.InnerText.StartsWith("Born:"))
                    {
                        var str = node.InnerText
                            .Replace("Born:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();


                        var matches = Regex.Matches(str, @"\d+");
                        var mNames = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.ToList();

                        if (matches.Count == 2 &&
                            int.TryParse(matches[0].Value, out var day) &&
                            int.TryParse(matches[1].Value, out var year) &&
                            mNames.Any(x => !string.IsNullOrWhiteSpace(x) && Regex.IsMatch(str, x)))
                        {
                            var monthName = mNames.First(x => Regex.IsMatch(str, x));
                            var index = mNames.IndexOf(monthName);
                            var month = index + 1;
                            result.Geburtstag = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

                        }
                    }
                    else if (node.InnerText.StartsWith("Birthplace:"))
                    {
                        var str = node.InnerText
                            .Replace("Birthplace:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();

                        var parts = str.Split(',');
                        if (parts.Length == 1)
                        {
                            result.Land = parts[0].Trim();
                        }
                        else if (parts.Length == 2)
                        {
                            result.Geburtsort = parts[0].Trim();
                            result.Land = parts[1].Trim();
                        }
                    }
                    else if (node.InnerText.StartsWith("Profession:"))
                    {
                        result.Beruf = node.InnerText
                            .Replace("Profession:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.StartsWith("Ethnicity:"))
                    {
                        result.Rasse = node.InnerText
                            .Replace("Ethnicity:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.StartsWith("Hair color:"))
                    {
                        result.Haare = node.InnerText
                            .Replace("Hair color:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.StartsWith("Eye color:"))
                    {
                        result.Augen = node.InnerText
                            .Replace("Eye color:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.StartsWith("Measurements:"))
                    {
                        string temp = node.InnerText
                            .Replace("Measurements:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();

                        if (!string.IsNullOrWhiteSpace(temp))
                        {
                            string maße = ConvertMaßeToMetric(temp);
                            result.Maße = maße;

                            string cup = ConvertMaßeToCupSize(temp);
                            result.Körbchengröße = cup;
                        }

                    }
                    else if (node.InnerText.StartsWith("Height:"))
                    {
                        var str = node.InnerText
                                .Replace("Height:", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        var match = Regex.Match(str.Substring(str.IndexOf('(')), @"\d+");
                        if (match.Success)
                        {
                            result.Größe = int.Parse(match.Value);
                        }
                    }
                    else if (node.InnerText.StartsWith("Weight:"))
                    {
                        var str = node.InnerText
                                .Replace("Weight:", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        var match = Regex.Match(str.Substring(str.IndexOf('(')), @"\d+");
                        if (match.Success)
                        {
                            result.Gewicht = int.Parse(match.Value);
                        }
                    }
                    else if (node.InnerText.StartsWith("Years active:"))
                    {
                        var str = node.InnerText
                            .Replace("Years active:", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();

                        var matchStart = Regex.Match(str, @"\d+");
                        if (matchStart.Success && int.TryParse(matchStart.Value, out var year))
                        {
                            result.Karrierestart = new DateTime(year, 1, 1);
                        }
                        result.IncludeStillActive(str);
                    }
                }
            }



            return result;

        }



    }
}
