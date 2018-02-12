using aemarcoCore.Common;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntrySource
    {
        private Uri _baseUri;
        private HtmlNode _rootNode;
        private string _categoryName;

        private HtmlDocument _detailsDoc;
        private HtmlDocument _downloadDoc;
        private Uri _imageUri;
        private Uri _thumbUri;
        private string _filename;
        private string _extension;
        private IContentCategory _contentCategory;
        private List<string> _tags;

        internal WallEntrySource(Uri baseUri, HtmlNode rootNode, string categoryName)
        {
            _baseUri = baseUri;
            _rootNode = rootNode;
            _categoryName = categoryName;
        }


        internal HtmlNode RootNode
        {
            get { return _rootNode; }
        }
        internal HtmlDocument DetailsDoc
        {
            get { return _detailsDoc; }
            set { _detailsDoc = value; }
        }
        internal HtmlDocument DownloadDoc
        {
            get { return _downloadDoc; }
            set { _downloadDoc = value; }
        }


        internal Uri ImageUri
        {
            get { return _imageUri; }
            set { _imageUri = value; }
        }
        internal Uri ThumbnailUri
        {
            get { return _thumbUri; }
            set { _thumbUri = value; }
        }
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }
        public IContentCategory ContentCategory
        {
            get { return _contentCategory; }
            set { _contentCategory = value; }
        }
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }


        internal WallEntry WallEntry
        {
            get
            {
                var entry = new WallEntry(_imageUri.AbsoluteUri, _thumbUri.AbsoluteUri,
                    _filename, _extension, _contentCategory, _categoryName, _tags);
                if (entry.IsValid)
                {
                    return entry;
                }
                else
                {
                    return null;
                }
            }
        }




        internal HtmlDocument GetDetailsDocFromNode(HtmlNode node, string nodeToDetailsNode = null)
        {
            HtmlNode subNode;
            if (nodeToDetailsNode == null)
            {
                subNode = node;
            }
            else
            {
                subNode = node?.SelectSingleNode(nodeToDetailsNode);
            }
            var href = subNode?.Attributes["href"]?.Value;
            var uri = new Uri(_baseUri, href);
            return new HtmlWeb().Load(uri);
        }

        internal HtmlDocument GetChildDocument(HtmlDocument doc, string docToHrefNode)
        {
            var node = doc?.DocumentNode.SelectSingleNode(docToHrefNode);
            var href = node?.Attributes["href"]?.Value;
            var uri = new Uri(_baseUri, href);
            return new HtmlWeb().Load(uri);
        }

        internal Uri GetUriFromDocument(HtmlDocument doc, string docToTargetNode, string attribute)
        {
            var node = doc?.DocumentNode.SelectSingleNode(docToTargetNode);
            var href = node?.Attributes[attribute]?.Value;
            return new Uri(_baseUri, href);
        }

        
        internal (string filename, string extension) GetFileDetails(Uri imageUri, string prefix = null)
        {
            string pref = (prefix == null) ? string.Empty : $"{prefix}_";

            string url = imageUri.AbsoluteUri;
            string main = url?.Substring(url.LastIndexOf("/") + 1);
            string name = $"{pref}{main}";

            return (Path.GetFileNameWithoutExtension(name), Path.GetExtension(name));
        }

        internal (string filename, string extension) GetFileDetails(Uri imageUri, string beforeName, string afterName, string prefix = null)
        {
            string pref = (prefix == null) ? string.Empty : $"{prefix}_";

            string url = imageUri.AbsoluteUri;
            string after = url.Substring(url.IndexOf(beforeName) + beforeName.Length);
            string main = after.Substring(0, after.IndexOf(afterName));

            string name = $"{pref}{main}";

            return (name, Path.GetExtension(url));
        }

        internal List<string> GetTagsFromNode(HtmlNode node, string attribute, string nodeToTargetNode = null)
        {
            HtmlNode subNode;
            if (nodeToTargetNode == null)
            {
                subNode = node;
            }
            else
            {
                subNode = node?.SelectSingleNode(nodeToTargetNode);
            }
            var tagString = subNode?.Attributes[attribute]?.Value;

            return GetTagsFromTagString(tagString);
        }

        internal List<string> GetTagsFromNodes(HtmlDocument doc, string docToTargetNodes, Func<HtmlNode, string> selector)
        {
            var nodes = doc?.DocumentNode.SelectNodes(docToTargetNodes);
            var tags = nodes?.Select(selector).ToList();
            if (tags == null)
            {
                return new List<string>();
            }
            else
            {
                return tags;
            }
        }


        internal List<string> GetTagsFromTagString(string tagString)
        {
            //z.B. "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"
            List<string> result = new List<string>();

            if (String.IsNullOrWhiteSpace(tagString))
            {
                return result;
            }

            string[] tags = tagString.Split(',');
            foreach (string tag in tags)
            {
                //z.B. "flowerdress"
                string entry = tag.Trim();
                if (entry.Length > 0)
                {
                    result.Add(entry);
                }
            }
            return result;
        }


    }
}
