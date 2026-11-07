
# aemarcoWallpaperCrawler [![NuGet Version](https://img.shields.io/nuget/v/aemarcoWallpaperCrawler.svg?style=flat-square)](https://www.nuget.org/packages/aemarcoWallpaperCrawler)


Register with services.AddWallCrawler(), then resolve an IWallpaperCrawler for each crawl (transient, it holds the state of one crawl).

	No start- and lastpage input yield to automatic mode, where only News are crawled (till 10 pages or 10 known entries in sequence).

	Configure(...) may be used to specify known urls
	
	AddCategoryFilter(...) may be used to limit crawling to the desired categories.
	AddSourceSiteFilter(...) may be used to limit crawling to the desired site.
	Those filters can be combined :)

	call CrawlWallpapers(...)




