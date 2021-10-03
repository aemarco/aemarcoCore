using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using System;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{

    internal class PersonCrawlerPorngatherer : PersonCrawlerBase
    {
        public PersonCrawlerPorngatherer(string nameToCrawl, CancellationToken cancellationToken)
              : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://pornsites.xxx");
        internal override PersonSite PersonSite => PersonSite.Porngatherer;
        internal override int PersonPriority => 10;
        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            var href = $"pornstars/{NameToCrawl.Replace(' ', '-')}";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@id='main']/header/h1");
            var pictureNodes = document.DocumentNode.SelectNodes("//div[@class='pornstar-box-con-big']/div/div/picture/img");
            var nodeWithData = document.DocumentNode.SelectNodes("//table[@class='styled']/tr");
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
             if (pictureNodes != null)
                foreach(var pictureNode in pictureNodes)
                {
                    var attrib = pictureNode.Attributes["src"];
                    if (attrib == null) continue;
                    result.IncludeProfilePicture(attrib.Value);
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
                        var str = node.InnerText
                            .Replace("Age:", string.Empty)
                            .Trim();
                        str = str.Substring(0, str.IndexOf(' '));


                        if (DateTime.TryParse(str, out var dt))
                        {
                            result.Geburtstag = dt;
                        }
                    }
                    //not tested
                    else if (node.InnerText.Contains("Hair color:"))
                    {
                        result.Haare = node.InnerText.Replace("Hair color:", string.Empty).Trim();
                    }
                    //not tested
                    else if (node.InnerText.Contains("Eye color:"))
                    {
                        result.Augen = node.InnerText.Replace("Eye color:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Weight:"))
                    {
                        try
                        {
                            var str = node.InnerText.Replace("Weight:", string.Empty);
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
                            var str = node.InnerText.Replace("Height:", string.Empty);
                            str = str.Substring(str.IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf("cm)") - 1);
                            result.Größe = Convert.ToInt32(str);
                        }
                        catch { }
                    }
                    //not tested
                    else if (node.InnerText.Contains("Ethnicity:"))
                    {
                        result.Rasse = node.InnerText.Replace("Ethnicity:", string.Empty).Trim();
                    }
                    //not tested
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
