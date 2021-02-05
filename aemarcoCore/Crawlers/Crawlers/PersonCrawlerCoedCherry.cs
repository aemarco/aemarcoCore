using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerCoedCherry : PersonCrawlerBase
    {
        public PersonCrawlerCoedCherry(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://www.coedcherry.com");
        internal override PersonSite PersonSite => PersonSite.CoedCherry;
        internal override int PersonPriority => 25;

        protected override int MinDelay => 400;
        protected override int MaxDelay => 2000;

        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            // /models/foxy-di/biography
            var href = $"/models/{NameToCrawl.Replace(' ', '-')}/biography";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='submenu-header']/h1");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//table[@class='table table-responsive table-striped']");

            //Name
            if (nodeWithName != null)
            {
                var n = nodeWithName.InnerText.Trim();
                n = n.Replace("'s Bio", string.Empty);
                n = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(n.ToLower());
                if (n.Contains(" "))
                {
                    result.FirstName = n.Substring(0, n.IndexOf(' '));
                    result.LastName = n.Substring(n.IndexOf(' ') + 1);
                }
            }

            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    //skipping
                    if (node.Name != "tr") continue;
                    if (node.InnerText.Contains(" Sites") || node.InnerText.Contains("About")) continue;

                    //Geburtstag
                    if (node.InnerText.Contains("Birthday"))
                    {
                        var str = node.InnerText
                            .Replace("Birthday", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                        if (str.Contains("("))
                            str = str.Substring(0, str.IndexOf("("));



                        if (DateTime.TryParse(str, out var dt))
                        {
                            result.Geburtstag = dt;
                        }
                    }
                    //Land
                    else if (node.InnerText.Contains("Country") && !node.InnerText.Contains("State/Country"))
                    {
                        result.Land = node.InnerText
                            .Replace("Country", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Place of Birth"))
                    {
                        result.Geburtsort = node.InnerText
                            .Replace("Place of Birth", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                        if (string.IsNullOrWhiteSpace(result.Geburtsort)) result.Geburtsort = null;
                    }
                    else if (node.InnerText.Contains("Profession"))
                    {
                        result.Beruf = node.InnerText
                            .Replace("Profession", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Status"))
                    {
                        var status = node.InnerText
                            .Replace("Status", string.Empty)
                            .Replace("\n", string.Empty);
                        result.IncludeStillActive(status);
                    }
                    //Aliase
                    else if (node.InnerText.Contains("Aliases"))
                    {

                        //Aliases
                        //Nensi B, Medina U, Foxy Di, Nensi B Medina, Kate X-Art, Foxi Di, Katoa Erotic Beauty, Nensi Amour Angels, Nensi Show Beauty, Katoa Errotica Archives
                        var aliasString = node.InnerText;

                        //Nensi B, Medina U, Foxy Di, Nensi B Medina, Kate X-Art, Foxi Di, Katoa Erotic Beauty, Nensi Amour Angels, Nensi Show Beauty, Katoa Errotica Archives
                        aliasString = aliasString.Replace("Aliases", string.Empty);
                        aliasString = aliasString.Replace("\n", string.Empty);
                        aliasString = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(aliasString.ToLower());
                        if (aliasString.EndsWith("."))
                            aliasString = aliasString.Remove(aliasString.Length - 1);
                        //Nensi B, Medina U, Foxy Di, Nensi B Medina, Kate X-Art, Foxi Di, Katoa Erotic Beauty, Nensi Amour Angels, Nensi Show Beauty, Katoa Errotica Archives
                        foreach (var aliasItem in aliasString.Split(','))
                        {
                            var al = aliasItem.Trim();
                            if (al.StartsWith(".")) al = al.Remove(0, 1);
                            al = al.Trim();

                            if (al.Length > 3 && al.Contains(" "))
                            {
                                result.Aliase.Add(al);
                            }
                        }
                        //remove original name if its under aliases
                        result.Aliase.Remove($"{result.FirstName} {result.LastName}");

                    }
                    else if (node.InnerText.Contains("Ethnicity"))
                    {
                        result.Rasse = node.InnerText
                            .Replace("Ethnicity", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Hair Colour"))
                    {
                        result.Haare = node.InnerText
                            .Replace("Hair Colour", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Eye Colour"))
                    {
                        result.Augen = node.InnerText
                            .Replace("Eye Colour", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Measurements"))
                    {
                        var temp = node.InnerText
                            .Replace("Measurements", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();

                        if (!string.IsNullOrWhiteSpace(temp))
                        {
                            var maße = ConvertMaßeToMetric(temp, true);
                            result.Maße = maße;

                            var cup = ConvertMaßeToCupSize(temp);
                            result.Körbchengröße = cup;
                        }

                    }
                    else if (node.InnerText.Contains("Height"))
                    {
                        var str = node.InnerText
                                .Replace("Height", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        result.Größe = ConvertFeetAndInchToCm(str);
                    }
                    else if (node.InnerText.Contains("Weight"))
                    {
                        var str = node.InnerText
                                .Replace("Weight", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        result.Gewicht = ConvertLibsToKg(str);
                    }
                    else if (node.InnerText.Contains("Piercings"))
                    {
                        var pierc = node.InnerText
                            .Replace("Piercings", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                        pierc = pierc.Replace("None", string.Empty).Trim();
                        if (!string.IsNullOrWhiteSpace(pierc))
                        {
                            result.Piercings = pierc;
                        }
                    }


                }
            }



            return result;

        }


















    }
}
