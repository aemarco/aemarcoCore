﻿namespace aemarco.Crawler.Wallpaper.Model;

internal class CrawlOffer
{
    #region ctor

    public CrawlOffer(
        int startPage,
        int lastPage,
        bool onlyNews,
        string siteCategoryName,
        PageUri categoryUri,
        ContentCategory category)
    {
        _startPage = startPage;
        _lastPage = lastPage;
        _onlyNews = onlyNews;

        CurrentPage = _startPage;

        SiteCategoryName = siteCategoryName;
        CategoryUri = categoryUri;
        Category = category;
    }



    #endregion

    #region props

    //kind of crawl
    public CrawlMethod CrawlMethod { get; set; } = CrawlMethod.Classic;
    public string SiteCategoryName { get; }
    public PageUri CategoryUri { get; }
    public ContentCategory Category { get; }

    //paging
    public int CurrentPage { get; private set; }
    public int DoneEntries { get; private set; }
    public int TotalEntries { get; private set; }




    private int _lastPage;
    public bool ContainsPagesToDo => CurrentPage <= _lastPage;


    private readonly bool _onlyNews;
    public bool ReachedMaximumStreak => (_onlyNews && _numberOfEntriesKnownInSequence >= 10);

    #endregion

    #region methods internal


    private readonly int _startPage;
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