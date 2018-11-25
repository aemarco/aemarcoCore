using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerPorngatherer : PersonCrawlerBasis
    {
        public PersonCrawlerPorngatherer(string nameToCrawl, CancellationToken cancellationToken)
              : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://www.pornsitestars.com");

        internal override PersonEntry GetPersonEntry()
        {
            PersonEntry result = new PersonEntry(_nameToCrawl)
            {
                PersonEntrySource = "Porngatherer",
                PersonEntryPriority = 10
            };

            string href = $"de/pornstars/{_nameToCrawl.Replace(' ', '-')}";
            Uri target = new Uri(_uri, href);
            HtmlDocument document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='tab-left lefttext']");
            var nodeWithBild = document.DocumentNode.SelectSingleNode("//div[@class='fallbox']")?.FirstChild?.FirstChild;
            var nodeWithAliase = document.DocumentNode.SelectSingleNode("//div[@class='center']")?.NextSibling;
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

            //Aliase
            if (nodeWithAliase != null && nodeWithAliase.InnerText.StartsWith("aka"))
            {
                string aliasString = nodeWithAliase.InnerText;
                //aka Nensi B, Medina U, Foxy Di, Nensi B Medina, Kate X-Art, Foxi Di, Katoa Erotic Beauty, Nensi Amour Angels, Nensi Show Beauty
                aliasString = aliasString.Substring(4);
                aliasString = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(aliasString.ToLower());
                // Nensi B, Medina U, Foxy Di, Nensi B Medina, Kate X-Art, Foxi Di, Katoa Erotic Beauty, Nensi Amour Angels, Nensi Show Beauty
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

            return result;
        }
    }
}
