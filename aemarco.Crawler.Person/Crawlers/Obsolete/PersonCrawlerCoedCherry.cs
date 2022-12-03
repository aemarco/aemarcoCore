using aemarco.Crawler.Person.Common;
using aemarco.Crawler.Person.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.Person.Crawlers.Obsolete
{
    [PersonCrawler("CoedCherry", 25)]
    internal class PersonCrawlerCoedCherry : PersonCrawlerBase
    {

        public PersonCrawlerCoedCherry(string nameToCrawl)
            : base(nameToCrawl)
        { }

        private readonly Uri _uri = new Uri("https://www.coedcherry.com");





        internal override Task<PersonInfo> GetPersonEntry(CancellationToken cancellationToken)
        {
            var result = new PersonInfo(this);

            // /models/foxy-di/biography
            var href = $"/models/{NameToCrawl.Replace(' ', '-')}/biography";
            var target = new Uri(_uri, href);
            var document = HtmlHelper.GetHtmlDocument(target, 400, 2000);
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
                    cancellationToken.ThrowIfCancellationRequested();

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
                            str = str.Substring(0, str.IndexOf("(", StringComparison.Ordinal));



                        if (DateTime.TryParse(str, out var dt))
                        {
                            result.Birthday = dt;
                        }
                    }
                    //Land
                    else if (node.InnerText.Contains("Country") && !node.InnerText.Contains("State/Country"))
                    {
                        result.Country = node.InnerText
                            .Replace("Country", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Place of Birth"))
                    {
                        result.City = node.InnerText
                            .Replace("Place of Birth", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                        if (string.IsNullOrWhiteSpace(result.City)) result.City = null;
                    }
                    else if (node.InnerText.Contains("Profession"))
                    {
                        result.Profession = node.InnerText
                            .Replace("Profession", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Status"))
                    {
                        var status = node.InnerText
                            .Replace("Status", string.Empty)
                            .Replace("\n", string.Empty);
                        result.StillActive = IsStillActive(status);
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
                                result.Aliases.Add(al);
                            }
                        }
                        //remove original name if its under aliases
                        result.Aliases.Remove($"{result.FirstName} {result.LastName}");

                    }
                    else if (node.InnerText.Contains("Ethnicity"))
                    {
                        result.Ethnicity = node.InnerText
                            .Replace("Ethnicity", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Hair Colour"))
                    {
                        result.HairColor = node.InnerText
                            .Replace("Hair Colour", string.Empty)
                            .Replace("\n", string.Empty)
                            .Trim();
                    }
                    else if (node.InnerText.Contains("Eye Colour"))
                    {
                        result.EyeColor = node.InnerText
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
                            result.Measurements = maße;

                            var cup = ConvertMaßeToCupSize(temp);
                            result.CupSize = cup;
                        }

                    }
                    else if (node.InnerText.Contains("Height"))
                    {
                        var str = node.InnerText
                                .Replace("Height", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        result.Height = ConvertFeetAndInchToCm(str);
                    }
                    else if (node.InnerText.Contains("Weight"))
                    {
                        var str = node.InnerText
                                .Replace("Weight", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                        result.Weight = ConvertLibsToKg(str);
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



            return Task.FromResult(result);

        }


















    }
}
