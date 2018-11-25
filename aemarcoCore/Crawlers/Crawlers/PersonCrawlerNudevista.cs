﻿using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Threading;


namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerNudevista : PersonCrawlerBasis
    {

        public PersonCrawlerNudevista(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("http://www.nudevista.at");

        internal override PersonEntry GetPersonEntry()
        {
            PersonEntry result = new PersonEntry(_nameToCrawl)
            {
                PersonEntrySource = "Nudevista",
                PersonEntryPriority = 20
            };

            string href = $"?q={_nameToCrawl.Replace(' ', '+')}&s=s";
            Uri target = new Uri(_uri, href);
            HtmlDocument document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//td[contains(@valign, 'top') and contains(@colspan ,'2')]");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//img[@class='mthumb']");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//div[@id='params_scroll']");

            //Name
            if (nodeWithName != null)
            {
                string n = nodeWithName.InnerText.Trim();
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
                string address = nodeWithBild.Attributes["src"].Value;
                if (!address.StartsWith("http"))
                {
                    address = "http:" + address;
                }
                result.PictureUrl = address;
                result.PictureSuggestedAdultLevel = -1;
            }

            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    //Geburtstag
                    if (node.InnerText.Contains("Geburtstag:"))
                    {
                        string str = node.InnerText.Replace("Geburtstag: ", "").Trim();
                        if (DateTime.TryParse(str, out DateTime dt))
                        {
                            result.Geburtstag = dt;
                        }
                    }
                    //Land
                    else if (node.InnerText.Contains("Land:"))
                    {
                        result.Land = node.InnerText.Replace("Land:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Geburtsort:"))
                    {
                        result.Geburtsort = node.InnerText.Replace("Geburtsort:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Beruf:"))
                    {
                        result.Beruf = node.InnerText.Replace("Beruf:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Karrierestart:"))
                    {
                        try
                        {
                            string str = node.InnerText.Replace("Karrierestart:", "").Trim();
                            str = str.Replace("-", "").Trim();

                            result.Karrierestart = new DateTime(Convert.ToInt32(str), 1, 1);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Karrierestatus:"))
                    {
                        result.Karrierestatus = node.InnerText.Replace("Karrierestatus:", "").Trim();
                    }
                    //Aliase
                    else if (node.InnerText.Contains("Auch bekannt als"))
                    {
                        //&& item.Aliase == null


                        string aliasString = node.InnerText;
                        //Auch bekannt als Becky Lesabre, Beth Porter.
                        aliasString = node.InnerText.Replace("Auch bekannt als", "");
                        aliasString = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(aliasString.ToLower());
                        // Becky Lesabre, Beth Porter.
                        if (aliasString.EndsWith("."))
                            aliasString = aliasString.Remove(aliasString.Length - 1);
                        // Becky Lesabre, Beth Porter
                        foreach (string aliasItem in aliasString.Split(','))
                        {
                            var al = aliasItem.Trim();
                            if (al.Length > 3 && al.Contains(" "))
                            {
                                result.Aliase.Add(al);
                            }

                        }
                    }
                    else if (node.InnerText.Contains("Rasse:"))
                    {
                        result.Rasse = node.InnerText.Replace("Rasse:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Haare:"))
                    {
                        result.Haare = node.InnerText.Replace("Haare:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Augen:"))
                    {
                        result.Augen = node.InnerText.Replace("Augen:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Maße:"))
                    {
                        result.Maße = node.InnerText.Replace("Maße:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Körbchengröße:"))
                    {
                        result.Körbchengröße = node.InnerText.Replace("Körbchengröße:", "").Trim();
                    }
                    else if (node.InnerText.Contains("Größe:"))
                    {
                        try
                        {
                            string str = node.InnerText;
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
                            string str = node.InnerText;
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("kg)") - 1);
                            result.Gewicht = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Piercings:"))
                    {
                        var pierc = node.InnerText.Replace("Piercings:", "").Trim();
                        pierc = pierc.Replace("None", "").Trim();
                        if (!String.IsNullOrWhiteSpace(pierc))
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
