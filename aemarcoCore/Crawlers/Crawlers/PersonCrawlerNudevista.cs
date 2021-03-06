﻿using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using System;
using System.Threading;


namespace aemarcoCore.Crawlers.Crawlers
{

    internal class PersonCrawlerNudevista : PersonCrawlerBase
    {

        public PersonCrawlerNudevista(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://www.nudevista.at");
        internal override PersonSite PersonSite => PersonSite.Nudevista;
        internal override int PersonPriority => 20;
        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            var href = $"?q={NameToCrawl.Replace(' ', '+')}&s=s";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//td[contains(@valign, 'top') and contains(@colspan ,'2')]");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//img[@class='mthumb']");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//div[@id='params_scroll']");

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

            //Bild
            if (nodeWithBild != null &&
                nodeWithBild.Attributes["src"] != null)
            {
                var address = nodeWithBild.Attributes["src"].Value;
                if (!address.StartsWith("http"))
                {
                    address = "https:" + address;
                }

                result.IncludeProfilePicture(address);


               
            }

            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    //Geburtstag
                    if (node.InnerText.Contains("Geburtstag:"))
                    {
                        var str = node.InnerText.Replace("Geburtstag: ", string.Empty).Trim();
                        if (DateTime.TryParse(str, out var dt))
                        {
                            result.Geburtstag = dt;
                        }
                    }
                    //Land
                    else if (node.InnerText.Contains("Land:"))
                    {
                        result.Land = node.InnerText.Replace("Land:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Geburtsort:"))
                    {
                        result.Geburtsort = node.InnerText.Replace("Geburtsort:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Beruf:"))
                    {
                        result.Beruf = node.InnerText.Replace("Beruf:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Karrierestart:"))
                    {
                        try
                        {
                            var str = node.InnerText.Replace("Karrierestart:", string.Empty).Trim();
                            str = str.Replace("-", string.Empty).Trim();

                            result.Karrierestart = new DateTime(Convert.ToInt32(str), 1, 1);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Karrierestatus:"))
                    {
                        var str = node.InnerText
                            .Replace("Karrierestatus:", string.Empty)
                            .Trim();

                        result.IncludeStillActive(str);
                    }
                    //Aliase
                    else if (node.InnerText.Contains("Auch bekannt als"))
                    {
                        var aliasString = node.InnerText;
                        //Auch bekannt als Becky Lesabre, Beth Porter.
                        aliasString = node.InnerText.Replace("Auch bekannt als", string.Empty);
                        aliasString = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(aliasString.ToLower());
                        // Becky Lesabre, Beth Porter.
                        if (aliasString.EndsWith("."))
                            aliasString = aliasString.Remove(aliasString.Length - 1);
                        // Becky Lesabre, Beth Porter
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
                    }
                    else if (node.InnerText.Contains("Rasse:"))
                    {
                        result.Rasse = node.InnerText.Replace("Rasse:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Haare:"))
                    {
                        result.Haare = node.InnerText.Replace("Haare:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Augen:"))
                    {
                        result.Augen = node.InnerText.Replace("Augen:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Maße:"))
                    {
                        var temp = node.InnerText.Replace("Maße:", string.Empty).Trim();
                        var maße = ConvertMaßeToMetric(temp);
                        result.Maße = maße;
                    }
                    else if (node.InnerText.Contains("Körbchengröße:"))
                    {
                        result.Körbchengröße = node.InnerText.Replace("Körbchengröße:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Größe:"))
                    {
                        try
                        {
                            var str = node.InnerText;
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("cm)") - 1);
                            result.Größe = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Gewicht:"))
                    {
                        try
                        {
                            var str = node.InnerText;
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("kg)") - 1);
                            result.Gewicht = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Piercings:"))
                    {
                        var pierc = node.InnerText.Replace("Piercings:", string.Empty).Trim();
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
