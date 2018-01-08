# aemarcoCore

always use namespace aemarcoCore.Common

Crawlers: 
in namespace aemarcoCore.Crawler

Usage:
Create any Crawler object derived from WallpaperCrawlerBasis and start it.
Start(), returns IWallCrawlerResult once completed
StartAsync(), fire and forget. Events may be used to capture Results.
StartAsyncTask(), returns Task<IWallCrawlerResult>.
Events deliver either IWallEntry or IWallCrawlerResult, both of which provide JSON options.
Progress event deliver the current progress 0...100
  
Options:
Setting File inlcudes:
-CrawlerFileExtensions limitation for limitation of FileExtensions.
-CrawlerData for Definition of a datapath for saving known stuff.
By constructor following behaviour:
-No start- and lastpage input yield to automatic mode, where only News are crawled (till 10 pages or 10 known entries in sequence)
-IProgress may be tossed in, to report Progress
-CancellationToken may be tossed in, for cancellation
-DirectoryInfo my be tossed in, where the IWallCrawlerResult will be saved as .json



