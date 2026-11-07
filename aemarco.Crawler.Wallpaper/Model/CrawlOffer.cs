namespace aemarco.Crawler.Wallpaper.Model;

internal class CrawlOffer
{
    #region ctor

    public CrawlOffer(
        string siteCategoryName,
        PageUri categoryUri,
        ContentCategory category)
    {
        SiteCategoryName = siteCategoryName;
        CategoryUri = categoryUri;
        Category = category;
    }

    /// <summary>
    /// offers exist before a crawl starts (filtering, available categories), paging comes with the start
    /// </summary>
    internal void SetPaging(int startPage, int lastPage, bool onlyNews)
    {
        _startPage = startPage;
        _lastPage = lastPage;
        _onlyNews = onlyNews;

        CurrentPage = _startPage;
    }



    #endregion

    #region props

    public string SiteCategoryName { get; }
    public PageUri CategoryUri { get; }
    public ContentCategory Category { get; }

    //paging
    public int CurrentPage { get; private set; }
    public int DoneEntries { get; private set; }
    public int TotalEntries { get; private set; }




    private int _lastPage;
    public bool ContainsPagesToDo => CurrentPage <= _lastPage;


    private bool _onlyNews;
    public bool ReachedMaximumStreak => (_onlyNews && _numberOfEntriesKnownInSequence >= 10);

    #endregion

    #region staging

    //what the offer found so far; it joins the crawl result only when the whole offer went through,
    //entries come newest first, so a half done offer would leave a gap behind the next crawl's known streak
    public List<WallEntry> NewEntries { get; } = [];
    public List<WallEntry> KnownEntries { get; } = [];
    public List<AlbumEntry> NewAlbums { get; } = [];
    public List<AlbumEntry> KnownAlbums { get; } = [];
    public HashSet<string> FoundUrls { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// drops everything the offer found
    /// </summary>
    /// <returns>number of entries dropped (albums count with their entries)</returns>
    internal int Discard()
    {
        var result = NewEntries.Count +
                     KnownEntries.Count +
                     NewAlbums.Sum(x => x.Entries.Count) +
                     KnownAlbums.Sum(x => x.Entries.Count);
        NewEntries.Clear();
        KnownEntries.Clear();
        NewAlbums.Clear();
        KnownAlbums.Clear();
        FoundUrls.Clear();
        return result;
    }

    #endregion

    #region methods internal


    private int _startPage;
    private int _numberOfEntriesPerPage;
    internal void ReportNumberOfEntriesPerPage(int count)
    {
        var numberOfPages = _lastPage - _startPage + 1;
        TotalEntries = numberOfPages * count;
        _numberOfEntriesPerPage = count;
    }



    private int _numberOfEntriesDoneForCurrentPage;
    internal void ReportPageDone()
    {
        CurrentPage++;
        _numberOfEntriesDoneForCurrentPage = 0;
    }


    private int _numberOfEntriesKnownInSequence;
    internal void ReportEntryDone(bool known)
    {
        if (known)
        {
            _numberOfEntriesKnownInSequence++;
        }
        else
        {
            _numberOfEntriesKnownInSequence = 0;
        }


        _numberOfEntriesDoneForCurrentPage++;

        var countBefore = (CurrentPage - _startPage) * _numberOfEntriesPerPage;
        DoneEntries = countBefore + _numberOfEntriesDoneForCurrentPage;
    }

    internal void ReportEndReached()
    {
        TotalEntries = DoneEntries;
        _lastPage = CurrentPage - 1;
    }



    #endregion


}