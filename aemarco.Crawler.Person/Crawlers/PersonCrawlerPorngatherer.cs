using aemarco.Crawler.Person.Common;
using aemarco.Crawler.Person.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.Person.Crawlers
{

    [PersonCrawler("Porngatherer", 10)]
    internal class PersonCrawlerPorngatherer : PersonCrawlerBase
    {
        public PersonCrawlerPorngatherer(string nameToCrawl)
              : base(nameToCrawl)
        { }

        private readonly Uri _uri = new Uri("https://pornsites.xxx");


        internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
        {
            var result = new PersonInfo(this);

            var href = $"pornstars/{NameToCrawl.Replace(' ', '-')}";
            var target = new Uri(_uri, href);
            var document = HtmlHelper.GetHtmlDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@id='main']/header/h1");
            var pictureNodes = document.DocumentNode.SelectNodes("//div[@class='pornstar-box-con-big']/div[@class='pornstar-box']/picture/img");
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
                foreach (var pictureNode in pictureNodes)
                {
                    var attrib = pictureNode.Attributes["src"];
                    if (attrib == null) continue;

                    result.ProfilePictures.Add(ProfilePicture.FromUrl(attrib.Value));
                }



            //data


            //Data
            if (nodeWithData != null)
            {
                foreach (var node in nodeWithData)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    //Geburtstag
                    if (node.InnerText.Contains("Age:"))
                    {
                        var str = node.InnerText
                            .Replace("Age:", string.Empty)
                            .Trim();
                        str = str.Substring(0, str.IndexOf(' '));


                        if (DateTime.TryParse(str, out var dt))
                        {
                            result.Birthday = dt;
                        }
                    }
                    //not tested
                    else if (node.InnerText.Contains("Hair color:"))
                    {
                        result.HairColor = node.InnerText.Replace("Hair color:", string.Empty).Trim();
                    }
                    //not tested
                    else if (node.InnerText.Contains("Eye color:"))
                    {
                        result.EyeColor = node.InnerText.Replace("Eye color:", string.Empty).Trim();
                    }
                    else if (node.InnerText.Contains("Weight:"))
                    {
                        try
                        {
                            var str = node.InnerText.Replace("Weight:", string.Empty);
                            str = str.Substring(str.IndexOf("(", StringComparison.Ordinal) + 1);
                            str = str.Substring(0, str.IndexOf("kg)", StringComparison.Ordinal) - 1);
                            result.Weight = Convert.ToInt32(str);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    else if (node.InnerText.Contains("Height:"))
                    {
                        try
                        {
                            var str = node.InnerText.Replace("Height:", string.Empty);
                            str = str.Substring(str.IndexOf("(", StringComparison.Ordinal) + 1);
                            str = str.Substring(0, str.IndexOf("cm)", StringComparison.Ordinal) - 1);
                            result.Height = Convert.ToInt32(str);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    //not tested
                    else if (node.InnerText.Contains("Ethnicity:"))
                    {
                        result.Ethnicity = node.InnerText.Replace("Ethnicity:", string.Empty).Trim();
                    }
                    //not tested
                    else if (node.InnerText.Contains("Country:"))
                    {
                        result.Country = node.InnerText.Replace("Country:", string.Empty).Trim();
                    }
                }
            }



            return Task.FromResult(result);
        }
    }
}
