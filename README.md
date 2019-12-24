# aemarcoCore

[![Build Status](https://dev.azure.com/aemarco/aemarcoCentral/_apis/build/status/aemarco.aemarcoCore?branchName=master)](https://dev.azure.com/aemarco/aemarcoCentral/_build/latest?definitionId=9&branchName=master)


Breaking changes may ocure(independend of version), even with already done code and interfaces.
always use namespace aemarcoCore.Common

Crawler Usage:
in namespace aemarcoCore.Crawler
Create a WallpaperCrawler for usage.

	No start- and lastpage input yield to automatic mode, where only News are crawled (till 10 pages or 10 known entries in sequence).
	IProgress may be tossed in, to report Progress.
	CancellationToken may be tossed in, for cancellation.

	ReportPath can be set to receive a file report at the set Destination
	AddCategoryFilter may be used to limit crawling to the desired categories.
	AddSourceSiteFilter may be used to limit crawling to the desired site.
	Those filters can be combined :)


	Start(), StartAsync(), StartAsyncTask() are availabe for starting the thing.

	Progress event delivers the current progress 0...100, safe for GUI-Use
	NewEntry and KnownEntry events deliver IWallEntry.
	
	Completed event delivers IWallCrawlerResult on completion.
	Beside NewEntries and KnownEntries it also holds AlbumEntries.
	AlbumEntries are IAlbumEntry which holds multiple IWallEntry´s collected as a album.
	Entries in Albums are not contained in New or Known Entries.... Album holds at least 1 new Entry
	

	Optional Configuration:
	Known Urls are either provided in ConfigurationHelper or handled via a json file containing a List<string>
	Sources for the list are handled in following order:
	ConfigurationHelper.KnownUrlsFunc --> known Urls are passed in via a Func
	ConfigurationHelper.KnownUrlsFile --> A file location which is beeing read from and written to	
	ApplicationDirectory subfolder JSON --> File 'known.json' will be used


Create a PersonCrawler for usage

	name in format 'FirstName LastName' to be tossed in the constructor.
	Start(), StartAsync(), StartAsyncTask() are availabe for starting the thing.

	IProgress, CancellationToken, ReportPath, Progress event, Entry event, Completed event..
	may be used in a similar fashion as the previous described crawler



