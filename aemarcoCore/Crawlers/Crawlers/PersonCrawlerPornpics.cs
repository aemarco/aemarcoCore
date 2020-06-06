using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading;

namespace aemarcoCore.Crawlers.Crawlers
{

    //this one is just crawling for profile pics

    internal class PersonCrawlerPornpics : PersonCrawlerBase
    {
        public PersonCrawlerPornpics(string nameToCrawl, CancellationToken cancellationToken)
            : base(nameToCrawl, cancellationToken)
        {
        }

        private readonly Uri _uri = new Uri("https://www.pornpics.com");
        internal override PersonSite PersonSite => PersonSite.Pornpics;
        internal override int PersonPriority => 5;
        internal override PersonEntry GetPersonEntry()
        {
            PersonEntry result = new PersonEntry(this);

            string href = $"/pornstars/list/";
            Uri target = new Uri(_uri, href);
            HtmlDocument document = GetDocument(target);

            var nodes = document.DocumentNode.SelectNodes("//div[@class='list-item']");
            var search = NameToCrawl.ToLower();

            var node = nodes?.FirstOrDefault(x => x.InnerText.ToLower().Contains(search));


            //Name
            if (node != null)
            {
                var nodeWithName = node.ChildNodes.FirstOrDefault(x => x.Name == "a");

                string n = nodeWithName.Attributes["title"]?.Value;
                n = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(n.ToLower());
                if (n.Contains(" "))
                {
                    result.FirstName = n.Substring(0, n.IndexOf(' '));
                    result.LastName = n.Substring(n.IndexOf(' ') + 1);
                }


                var nodeWithBild = node.ChildNodes.FirstOrDefault(x => x.Name == "i");
                string address = nodeWithBild.Attributes["rel"].Value;


                result.IncludeProfilePicture(address);
            }
            return result;

        }


    }
}
