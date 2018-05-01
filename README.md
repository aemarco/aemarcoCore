# aemarcoCore

Nuget: Be aware that the available Nuget package is always a bit behind with changes...

Frequent changes may ocure, even with already done code and interfaces.
always use namespace aemarcoCore.Common

Crawler Usage:
in namespace aemarcoCore.Crawler
Create a WallpaperCrawler for usage.

	No start- and lastpage input yield to automatic mode, where only News are crawled (till 10 pages or 10 known entries in sequence).
	IProgress may be tossed in, to report Progress.
	CancellationToken may be tossed in, for cancellation.

	ReportPath can be set to receive a file report at the set Destination
	AddCategoryFilter may be used to limit crawling to the desired categories.

	Start(), StartAsync(), StartAsyncTask() are availabe for starting the thing.

	Progress event delivers the current progress 0...100, safe for GUI-Use
	NewEntry and KnownEntry events deliver IWallEntry.
	Completed event delivers IWallCrawlerResult on completion.
	
	Optional Configuration:
	Known Urls are either provided in ConfigurationHelper or handled via a json file containing a List<string>
	Sources for the list are handled in following order:
	ConfigurationHelper.KnownUrlsFunc --> known Urls are passed in via a Func
	ConfigurationHelper.KnownUrlsFile --> A file location which is beeing readed and writed
	app.config CrawlerData --> A directory where a file 'known.json' will be used
	ApplicationDirectory subfolder JSON --> File 'known.json' will be used



WallpaperSetter Usage:
in namespace aemarcoCore.Wallpaper
Create a WallpaperSetter for usage

	WallpaperMode tossed in to determine behaviour

	CanBeSnapped can be used to find out if a Wallpaper can be snapped to the screen

	

