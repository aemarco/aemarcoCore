# aemarcoCore


Crawlers: 
in namespace aemarcoCore.Crawler

Usage:
Create any Crawler object derived from BildCrawlerBasis and start it.
Start(), returns ICrawlerResult once completed
StartAsync(), fire and forget. Events may be used to capture Results.
StartAsyncTask(), returns Task<ICrawlerResult>.
Events deliver either IWallEntry or ICrawlerResult, both of which provide GetJSON options.
  
Options:
Setting File inlcudes:
-CrawlerFileExtensions limitation for limitation of FileExtensions.
-CrawlerData for Definition of a datapath, where results will be saved.
By constructor following behaviour:
-No start- and lastpage input yield to automatic mode, where only News are crawled (till 10 pages or 10 known entries in sequence)
-IProgress may be tossed in, to report Progress
-CancellationToken may be tossed in, for cancellation



