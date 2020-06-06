using System;
using System.Linq;
using System.Threading;
using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using aemarcoCore.Crawlers.Types;
using HtmlAgilityPack;

namespace aemarcoCore.Crawlers.Crawlers
{
    internal class PersonCrawlerHookupHotshot : PersonCrawlerBase
    {
        public PersonCrawlerHookupHotshot(string nameToCrawl, CancellationToken cancellationToken) 
            : base(nameToCrawl, cancellationToken)
        { }

        

        private readonly Uri _uri = new Uri("https://hookuphotshot.com");
        internal override PersonSite PersonSite => PersonSite.HookupHotshot;
      
        internal override PersonEntry GetPersonEntry()
        {
            var result = new PersonEntry(this);

            // /girls/kenzie-reeves
            var href = $"/girls/{NameToCrawl.Replace(' ', '-')}";
            var target = new Uri(_uri, href);
            var document = GetDocument(target);
            var nodeWithName = document.DocumentNode.SelectSingleNode("//div[@class='girl-third girl-about']/h2");
            
            
            var nodesPictures = document.DocumentNode.SelectNodes("//img[@class='girl-pic']");
            
            
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


            //Picture
            if (nodesPictures?.FirstOrDefault() is HtmlNode nodeCute &&
                nodeCute.Attributes["src"].Value is string referenceCute)
            {
                if (!referenceCute.StartsWith("http")) referenceCute = "https:" + referenceCute;
               

                result.IncludeProfilePicture(referenceCute, 35, 39);
            }


            if (nodesPictures?.Skip(1).FirstOrDefault() is HtmlNode nodeDirty &&
                nodeDirty.Attributes["src"].Value is string referenceDirty)
            {
                if (!referenceDirty.StartsWith("http")) referenceDirty = "https:" + referenceDirty;
               

                result.IncludeProfilePicture(referenceDirty, 81, 89);
            }


            return result;

        }

    }
}
