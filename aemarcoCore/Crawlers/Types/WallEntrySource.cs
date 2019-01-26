using aemarcoCore.Common;
using aemarcoCore.Crawlers.Base;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntrySource
    {
        #region fields

        private readonly Uri _baseUri;
        private readonly HtmlNode _rootNode;
        private readonly string _categoryName;

        private HtmlDocument _detailsDoc;
        private HtmlDocument _downloadDoc;
        private Uri _imageUri;
        private Uri _thumbUri;
        private string _filename;
        private string _extension;
        private IContentCategory _contentCategory;
        private List<string> _tags;

        #endregion

        #region ctor

        internal WallEntrySource(Uri baseUri, HtmlNode rootNode, string categoryName)
        {
            _baseUri = baseUri;
            _rootNode = rootNode;
            _categoryName = categoryName;

        }

        #endregion

        #region props

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

        #endregion

        #region Output

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

        #endregion


        #region Documents

        internal HtmlDocument GetChildDocumentFromRootNode(string nodeToSubNode = null)
        {
            return GetChildDocumentFromNode(_rootNode, nodeToSubNode);
        }
        internal HtmlDocument GetChildDocumentFromNode(HtmlNode node, string nodeToSubNode = null)
        {
            HtmlNode subNode;
            if (nodeToSubNode == null)
            {
                subNode = node;
            }
            else
            {
                subNode = node?.SelectSingleNode(nodeToSubNode);
            }
            if (subNode == null)
            {
                return null;
            }
            var href = subNode?.Attributes["href"]?.Value;
            var uri = new Uri(_baseUri, href);
            return WallpaperCrawlerBasis.GetDocument(uri);
        }

        internal HtmlDocument GetChildDocumentFromDocument(HtmlDocument doc, string docToHrefNode)
        {
            var node = doc?.DocumentNode.SelectSingleNode(docToHrefNode);
            if (node != null)
            {
                return GetChildDocumentFromNode(node);

            }
            return null;
        }

        #endregion

        #region navigation

        internal Uri GetUriFromDocument(HtmlDocument doc, string docToTargetNode, string attribute)
        {
            var node = doc?.DocumentNode.SelectSingleNode(docToTargetNode);
            var href = node?.Attributes[attribute]?.Value;
            return new Uri(_baseUri, href);
        }
        internal string GetSubNodeAttribute(HtmlNode node, string attribute, string nodeToTargetNode = null)
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
            string value = subNode?.Attributes[attribute]?.Value;
            return value;
        }

        #endregion

        #region filedetails

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

        #endregion

        #region tags

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

        #endregion


    }
}
