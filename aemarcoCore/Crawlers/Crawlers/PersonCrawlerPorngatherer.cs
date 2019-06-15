using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
#pragma warning disable CRR0043 // Unused type
    internal class PersonCrawlerPorngatherer : PersonCrawlerBasis
    {
        public PersonCrawlerPorngatherer(string nameToCrawl, CancellationToken cancellationToken)
              : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://pornsites.xxx");

        internal override PersonEntry GetPersonEntry()
        {
            PersonEntry result = new PersonEntry(_nameToCrawl)
            {
                PersonEntrySource = "Porngatherer",
                PersonEntryPriority = 10
            };

            string href = $"pornstars/{_nameToCrawl.Replace(' ', '-')}";
            Uri target = new Uri(_uri, href);
            HtmlDocument document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='tab-left lefttext noborder mobilehide']");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//div[@class='fallboxcon']/a/div/img");
            var nodeWithData = document.DocumentNode.SelectNodes("//table[@class='styled']/tr");
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
                nodeWithBild.Attributes["data-src"] != null &&
                nodeWithBild.Attributes["data-src"].Value != "No Profile Picture")
            {
                string address = nodeWithBild.Attributes["data-src"].Value;
                result.PictureUrl = address;
                result.PictureSuggestedAdultLevel = -1;
            }

            //data


            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData)
                {
                    //Geburtstag
                    if (node.InnerText.Contains("Age:"))
                    {
                        string str = node.InnerText
                            .Replace("Age:", string.Empty)
                            .Trim();
                        str = str.Substring(0, str.IndexOf(' '));


                        if (DateTime.TryParse(str, out DateTime dt))
                        {
                            result.Geburtstag = dt;
                        }
                    }
                    else if (node.InnerText.Contains("Hair color:"))
                    {
                        result.Haare = node.InnerText.Replace("Hair color:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Eye color:"))
                    {
                        result.Augen = node.InnerText.Replace("Eye color:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Weight:"))
                    {
                        try
                        {
                            string str = node.InnerText.Replace("Weight:", string.Empty);
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("kg)") - 1);
                            result.Gewicht = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Height:"))
                    {
                        try
                        {
                            string str = node.InnerText.Replace("Height:", string.Empty);
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("cm)") - 1);
                            result.Größe = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Ethnicity:"))
                    {
                        result.Rasse = node.InnerText.Replace("Ethnicity:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Country:"))
                    {
                        result.Land = node.InnerText.Replace("Country:", string.Empty).Trim();
                    }
                }
            }



            return result;
        }
    }
}
