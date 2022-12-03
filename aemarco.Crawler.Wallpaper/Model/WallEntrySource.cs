using aemarco.Crawler.Wallpaper.Common;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace aemarco.Crawler.Wallpaper.Model
{
    internal class WallEntrySource
    {

        #region ctor

        private readonly Uri _baseUri;
        internal WallEntrySource()
        {

        }
        internal WallEntrySource(Uri baseUri, HtmlNode rootNode, string categoryName)
        {
            _baseUri = baseUri;
            RootNode = rootNode;
            SiteCategory = categoryName;

        }

        #endregion

        #region props

        internal HtmlNode RootNode { get; }
        internal HtmlDocument DetailsDoc { get; set; }
        internal HtmlDocument DownloadDoc { get; set; }

        #endregion

        #region Output

        internal Uri ImageUri { get; set; }
        internal Uri ThumbnailUri { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public ContentCategory ContentCategory { get; set; }
        public string SiteCategory { get; set; }
        public List<string> Tags { get; set; }
        public string AlbumName { get; set; }


        internal WallEntry WallEntry
        {
            get
            {
                var entry = new WallEntry(
                    ImageUri.AbsoluteUri,
                    ThumbnailUri.AbsoluteUri,
                    Filename,
                    Extension,
                    ContentCategory,
                    SiteCategory,
                    Tags,
                    AlbumName);

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

        internal HtmlDocument GetChildDocumentFromRootNode(string nodeToSubNode = null, int? minDelay = null, int? maxDelay = null)
        {
            return GetChildDocumentFromNode(RootNode, nodeToSubNode, minDelay, maxDelay);
        }
        internal HtmlDocument GetChildDocumentFromNode(HtmlNode node, string nodeToSubNode = null, int? minDelay = null, int? maxDelay = null)
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
            var href = subNode.Attributes["href"]?.Value;
            var uri = new Uri(_baseUri, href);
            return HtmlHelper.GetHtmlDocument(uri, minDelay, maxDelay);
        }
        internal HtmlDocument GetChildDocumentFromDocument(HtmlDocument doc, string docToHrefNode, int? minDelay = null, int? maxDelay = null)
        {
            var node = doc?.DocumentNode.SelectSingleNode(docToHrefNode);
            if (node != null)
            {
                return GetChildDocumentFromNode(node, null, minDelay, maxDelay);

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
            var value = subNode?.Attributes[attribute]?.Value;
            return value;
        }

        #endregion

        #region filedetails

        internal (string filename, string extension) GetFileDetails(Uri imageUri, string prefix = null)
        {
            var pref = (prefix == null) ? string.Empty : $"{prefix}_";

            var url = imageUri.AbsoluteUri;
            var main = url[(url.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
            main = WebUtility.HtmlDecode(main);
            var name = $"{pref}{main}";

            return (Path.GetFileNameWithoutExtension(name), Path.GetExtension(name));
        }
        internal (string filename, string extension) GetFileDetails(Uri imageUri, string beforeName, string afterName, string prefix = null)
        {
            var pref = (prefix == null) ? string.Empty : $"{prefix}_";

            var url = imageUri.AbsoluteUri;
            var after = url.Substring(url.IndexOf(beforeName, StringComparison.Ordinal) + beforeName.Length);
            var main = after.Substring(0, after.IndexOf(afterName, StringComparison.Ordinal));
            main = WebUtility.HtmlDecode(main);
            var name = $"{pref}{main}";

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
                tags.RemoveAll(x => x == null);
                return GetTagsFromTagString(string.Join(",", tags));
            }
        }

        internal List<string> GetTagsFromTagString(string tagString)
        {
            //z.B. "flowerdress, nadia p, susi r, suzanna, suzanna a, brunette, boobs, big tits"
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(tagString))
            {
                return result;
            }

            tagString = WebUtility.HtmlDecode(tagString).Trim();
            var tags = tagString.Split(',');
            foreach (var tag in tags)
            {
                //z.B. "flowerdress"
                var entry = tag.Trim();
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
