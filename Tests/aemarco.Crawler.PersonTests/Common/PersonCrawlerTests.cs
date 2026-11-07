namespace aemarco.Crawler.PersonTests.Common;

internal class PersonCrawlerTests
{

    [Test]
    public void AvailableCrawlers_ProvidesNames()
    {
        var sut = GetPersonCrawler();

        var result = sut.AvailableCrawlers
            .ToArray();

        result.ShouldBe(["Crawler1", "Crawler2"]);

        result.Dump();
    }

    [Test]
    public async Task AddPersonSiteFilter_Filters()
    {
        var sut = GetPersonCrawler();

        sut.AddPersonSiteFilter("Crawler1");

        //we check indirectly if StartAsync uses the filter
        var result = await sut.CrawlPerson("foxi", "di");
        result.CrawlerInfos.Count.ShouldBe(1);
        result.CrawlerInfos[0].FriendlyName.ShouldBe("Crawler1");

        result.CrawlerInfos.Dump();
    }

    [Test]
    public async Task CrawlPersonNames_Works()
    {
        //we ignore logging and errors here
        //we focus on deduplication and ordering

        var sut = GetPersonCrawler(true);

        var result = await sut.CrawlPersonNames(CancellationToken.None);

        result.ShouldBe([
            new PersonName("Ariel Rebel"),
            new PersonName("Foxi Di"),
            new PersonName("Piper Perri")]);

        result.Dump();
    }

    [Test]
    public async Task CrawlPerson_ShouldTitleName()
    {
        var sut = GetPersonCrawler();
        sut.AddPersonSiteFilter("Nope");

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Count.ShouldBe(0);
        result.FirstName.ShouldBe("Foxi");
        result.LastName.ShouldBe("Di");

        result.Dump();
    }

    [Test]
    public async Task CrawlPerson_ShouldUseCrawlers()
    {
        var sut = GetPersonCrawler();

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Count.ShouldBe(2);
        result.CrawlerInfos.Select(x => x.FriendlyName).ShouldBe(["Crawler1", "Crawler2"], true);

        result.CrawlerInfos.Dump();
    }

    [Test]
    public async Task CrawlPerson_ShouldHandleErrors()
    {
        var sut = GetPersonCrawler(true);

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Count.ShouldBe(2);
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Message.ShouldBe("Error");

        result.Errors.Select(x => x.Message).Dump();
    }

    [Test]
    public async Task CrawlPerson_ShouldMergeResult()
    {
        var sut = GetPersonCrawler();

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Count.ShouldBe(2);
        //order check as well
        result.CrawlerInfos[0].FriendlyName.ShouldBe("Crawler2");
        result.CrawlerInfos[1].FriendlyName.ShouldBe("Crawler1");

        result.CrawlerInfos.Dump();
    }

    //mock
    private static IPersonCrawler GetPersonCrawler(bool withError = false)
    {
        var crawler1 = Substitute.For<ISiteCrawler>();
        crawler1.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new PersonInfo
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Crawler1", 2) }
            });
        crawler1.GetPersonNameEntries(Arg.Any<CancellationToken>())
            .Returns(
            [
                new PersonName("Foxi Di"),
                new PersonName("Ariel Rebel")
            ]);


        var crawler2 = Substitute.For<ISiteCrawler>();
        crawler2.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new PersonInfo
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Crawler2", 1) }
            });
        crawler2.GetPersonNameEntries(Arg.Any<CancellationToken>())
            .Returns(
            [
                new PersonName("Piper Perri"),
                new PersonName("Ariel Rebel")
            ]);


        var crawler3 = Substitute.For<ISiteCrawler>();
        crawler3.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.Run(async () =>
            {
                await Task.Delay(100);
                throw new Exception("Error");
#pragma warning disable CS0162 // Unreachable code detected
                return new PersonInfo();
#pragma warning restore CS0162 // Unreachable code detected

            }));
        crawler3.GetPersonNameEntries(Arg.Any<CancellationToken>())
            .Returns(Task.Run(async () =>
            {
                await Task.Delay(100);
                throw new Exception("Error");
#pragma warning disable CS0162 // Unreachable code detected
                return Array.Empty<PersonName>();
#pragma warning restore CS0162 // Unreachable code detected
            }));


        var provider = Substitute.For<ISiteCrawlerProvider>();
        provider.GetAvailableCrawlerNames()
            .Returns(["Crawler1", "Crawler2"]);
        provider.GetFilteredCrawlerInstances(
                Arg.Any<string[]>())
            .Returns(call =>
            {
                var names = call.Arg<string[]>();
                List<ISiteCrawler> result = [crawler1, crawler2];
                if (withError)
                    result.Add(crawler3);

                return names.Length == 0
                    ? result
                        .ToArray()
                    : result
                        .Where(x => names
                            .Contains(x.GetPersonEntry("", "", CancellationToken.None).Result.CrawlerInfos[0].FriendlyName))
                        .ToArray();
            });



        var result = IocHelper.Resolve<IPersonCrawler>(sc =>
        {
            sc.RemoveAll<ISiteCrawlerProvider>();
            sc.AddSingleton(provider);
        });

        return result;
    }

}

