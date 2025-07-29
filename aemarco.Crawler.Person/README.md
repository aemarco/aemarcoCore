
# aemarcoPersonCrawler [![NuGet Version](https://img.shields.io/nuget/v/aemarcoPersonCrawler.svg?style=flat-square)](https://www.nuget.org/packages/aemarcoPersonCrawler)


Create a PersonCrawler for usage

    //register
    services.AddPersonCrawler();

    //crawling
    var svc = serviceProvider.GetRequiredService<IPersonCrawler>();
    await svc.CrawlPersonNames();
	await svc.CrawlPerson("firstName", "lastName");
	await svc.CrawlPerson("name");



