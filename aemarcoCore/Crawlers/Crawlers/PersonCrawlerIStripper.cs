using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerIStripper : PersonCrawlerBase
    {
        public PersonCrawlerIStripper(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://www.istripper.com");

        internal override PersonSite PersonSite => PersonSite.IStripper;
        internal override int PersonPriority => 30;

        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            var href = $"de/models/{NameToCrawl.Replace(' ', '-')}";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='trigger']/div/h1");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//div[@class='container']/img");
            var nodeWithData = document.DocumentNode.SelectSingleNode("//ul[@class='info2']");

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

                result.IncludeProfilePicture(address, 35, 39);

            }

            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData.ChildNodes)
                {
                    if (string.IsNullOrWhiteSpace(node.InnerText))
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
                            var str = node.InnerText.Replace("Größe", string.Empty).Trim();
                            str = str.Substring(0, str.IndexOf("cm") - 1).Trim();
                            result.Größe = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    else if (node.InnerText.Contains("Gewicht:"))
                    {
                        try
                        {
                            var str = node.InnerText.Replace("Gewicht:", string.Empty).Trim();
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
