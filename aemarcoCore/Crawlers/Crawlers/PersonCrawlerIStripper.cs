using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
#pragma warning disable CRR0043 // Unused type
    internal class PersonCrawlerIStripper : PersonCrawlerBasis
    {
        public PersonCrawlerIStripper(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://de.istripper.com");

        internal override PersonEntry GetPersonEntry()
        {
            PersonEntry result = new PersonEntry(_nameToCrawl)
            {
                PersonEntrySource = "IStripper",
                PersonEntryPriority = 30
            };

            string href = $"model/{_nameToCrawl.Replace(' ', '-')}";
            Uri target = new Uri(_uri, href);
            HtmlDocument document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='trigger']/div/h1");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//div[@class='container']/img");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//ul[@class='info2']");

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
                result.PictureUrl = address;
                result.PictureSuggestedAdultLevel = 39;

            }

            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    if (String.IsNullOrWhiteSpace(node.InnerText))
                    {
                        continue;
                    }
                    //Land
                    else if (node.InnerText.Contains("Land:"))
                    {
                        result.Land = node.InnerText.Replace("Land:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Stadt:"))
                    {
                        result.Geburtsort = node.InnerText.Replace("Stadt:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Maße:"))
                    {
                        result.Maße = node.InnerText.Replace("Maße:", string.Empty)
                            .Replace(" / ", "-")
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Größe"))
                    {
                        try
                        {
                            string str = node.InnerText.Replace("Größe", string.Empty).Trim();
                            str = str.Substring(0, str.IndexOf("cm") - 1).Trim();
                            result.Größe = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Gewicht:"))
                    {
                        try
                        {
                            string str = node.InnerText.Replace("Gewicht:", string.Empty).Trim();
                            str = str.Substring(0, str.IndexOf("kg") - 1).Trim();
                            result.Gewicht = Convert.ToInt32(str);
                        }
                        catch { }
                    }

                }
            }

            return result;

        }


    }
}
