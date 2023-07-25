namespace aemarco.Crawler.Wallpaper.Crawlers;

[WallpaperCrawler("Boombo")]
internal class Boombo : WallpaperCrawlerBasis
{

    private readonly Uri _uri = new("https://boombo.biz/");

    public Boombo(int startPage, int lastPage, bool onlyNews)
        : base(startPage, lastPage, onlyNews)
    {
    }

    protected override List<CrawlOffer> GetCrawlsOffers()
    {
        var result = new List<CrawlOffer>();
        var menu = new Uri(_uri, "en/");
        //main page
        var doc = HtmlHelper.GetHtmlDocument(menu);

        foreach (var node in doc.DocumentNode.SelectNodes("//div[@class='menu-popup']/ul/li/a"))
        {

            //z.B. "Nude photo"
            var text = WebUtility.HtmlDecode(node.InnerText).Trim();
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            //z.B. "/en/nude/
            var href = node.Attributes["href"]?.Value;
            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            //z.B. "https://ftopx.com/celebrities"
            var uri = new Uri(_uri, href);
            result.Add(CreateCrawlOffer(text, uri, new ContentCategory(Category.Girls)));
        }

        return result;
    }

    protected override Uri GetSiteUrlForCategory(CrawlOffer catJob)
    {
        //z.B. "https://boombo.biz/en/nude/page/1/"
        return new Uri(catJob.CategoryUri, $"page/{catJob.CurrentPage}/");
    }


    protected override string GetSearchStringGorEntryNodes()
    {
        return "//div[@class='short3']/a";
    }

    protected override bool AddWallEntry(PageNode pageNode, CrawlOffer catJob)
    {
        var node = pageNode.Node;
        var linkToAlbum = node.Attributes["href"]?.Value;
        var albumName = WallEntrySource.GetSubNodeInnerText(node, "./div[@class='reltit']");
        if (string.IsNullOrWhiteSpace(linkToAlbum) ||
            string.IsNullOrWhiteSpace(albumName))
            return false;

        var albumDoc = HtmlHelper.GetHtmlDocument(new Uri(linkToAlbum));

        var album = new AlbumEntry(albumName);

        var entryNodes = albumDoc.DocumentNode
            .SelectNodes("//a[@class='highslide']");
        if (entryNodes is not null && entryNodes.Count > 0)
        {
            foreach (var entryNode in entryNodes)
            {
                var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

                //details
                source.ImageUri = new PageUri(new Uri(entryNode.Attributes["href"].Value));
                if (source.ImageUri is null)
                {
                    AddWarning($"Could not get ImageUri from node {entryNode.InnerHtml}");
                    return false;
                }
                source.ThumbnailUri = source.ImageUri;
                (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
                source.ContentCategory = catJob.Category;
                source.Tags = source.GetTagsFromNode(
                    entryNode,
                    "alt", "./img");
                source.AlbumName = albumName;

                var wallEntry = source.WallEntry;
                if (wallEntry != null)
                {
                    album.Entries.Add(wallEntry);
                }
            }
        }



        var otherEntryNodes = albumDoc.DocumentNode
            .SelectNodes("//div[@class='text']/div/img");
        if (otherEntryNodes is not null && otherEntryNodes.Count > 0)
        {
            foreach (var entryNode in otherEntryNodes)
            {
                var source = new WallEntrySource(_uri, node, catJob.SiteCategoryName);

                //details
                source.ImageUri = new PageUri(new Uri(_uri, entryNode.Attributes["data-src"].Value));
                if (source.ImageUri is null)
                {
                    AddWarning($"Could not get ImageUri from node {entryNode.InnerHtml}");
                    return false;
                }
                source.ThumbnailUri = source.ImageUri;
                (source.Filename, source.Extension) = source.GetFileDetails(source.ImageUri);
                source.ContentCategory = catJob.Category;
                source.Tags = source.GetTagsFromNode(
                    entryNode,
                    "alt");
                source.AlbumName = albumName;

                var wallEntry = source.WallEntry;
                if (wallEntry != null)
                {
                    album.Entries.Add(wallEntry);
                }
            }
        }


        if (album.Entries.Any())
        {
            album.Entries.Sort(new WallEntryNumberComparer());
            AddAlbum(album, catJob);
        }
        return album.Entries.Any();
    }

    class WallEntryNumberComparer : IComparer<WallEntry>
    {
        public int Compare(WallEntry? x, WallEntry? y)
        {
            // Compare the length of the strings
            return GetNumber(x).CompareTo(GetNumber(y));
        }

        private int GetNumber(WallEntry? x)
        {
            if (x == null)
                return 0;

            var reg = Regex.Match(x.Url!, "(\\d+)(?!.*\\d)");
            if (reg.Success)
            {
                return int.Parse(reg.Groups[1].Value);
            }
            return 0;
        }
    }
}
