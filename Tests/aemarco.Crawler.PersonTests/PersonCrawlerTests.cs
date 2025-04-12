using NSubstitute;
using System.Threading;

namespace aemarco.Crawler.PersonTests;

internal class PersonCrawlerTests : TestBase
{

    [Test]
    public void AvailableCrawlers_ProvidesNames()
    {
        var sut = GetPersonCrawler();

        var result = sut.AvailableCrawlers
            .ToArray();

        result.Should().Equal("Crawler1", "Crawler2");

        PrintJson(result);
    }

    [Test]
    public async Task AddPersonSiteFilter_Filters()
    {
        var sut = GetPersonCrawler();

        sut.AddPersonSiteFilter("Crawler1");

        //we check indirectly if StartAsync uses the filter
        var result = await sut.StartAsync("foxi", "di");
        result.CrawlerInfos.Should().HaveCount(1);
        result.CrawlerInfos[0].FriendlyName.Should().Be("Crawler1");

        PrintJson(result.CrawlerInfos);
    }

    [Test]
    public async Task StartAsync_ShouldTitleName()
    {
        var sut = GetPersonCrawler();
        sut.AddPersonSiteFilter("Nope");

        var result = await sut.StartAsync("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(0);
        result.FirstName.Should().Be("Foxi");
        result.LastName.Should().Be("Di");

        PrintJson(result);
    }

    [Test]
    public async Task StartAsync_ShouldUseCrawlers()
    {
        var sut = GetPersonCrawler();

        var result = await sut.StartAsync("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        result.CrawlerInfos.Select(x => x.FriendlyName).Should().BeEquivalentTo("Crawler1", "Crawler2");

        PrintJson(result.CrawlerInfos);
    }

    [Test]
    public async Task StartAsync_ShouldHandleErrors()
    {
        var sut = GetPersonCrawler(true);

        var result = await sut.StartAsync("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Error");

        PrintJson(result.Errors);
    }


    [Test]
    public async Task StartAsync_ShouldMergeResult()
    {
        var sut = GetPersonCrawler();

        var result = await sut.StartAsync("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        //order check as well
        result.CrawlerInfos[0].FriendlyName.Should().Be("Crawler2");
        result.CrawlerInfos[1].FriendlyName.Should().Be("Crawler1");

        PrintJson(result.CrawlerInfos);
    }


    //mock
    private static PersonCrawler GetPersonCrawler(bool withError = false)
    {
        var crawler1 = Substitute.For<IPersonCrawler>();
        crawler1.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new PersonInfo
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Crawler1", 2) }
            });
        var crawler2 = Substitute.For<IPersonCrawler>();
        crawler2.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new PersonInfo
            {
                FirstName = "Foxi",
                LastName = "Di",
                CrawlerInfos = { new CrawlerInfo("Crawler2", 1) }
            });
        var crawler3 = Substitute.For<IPersonCrawler>();
        crawler3.GetPersonEntry(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.Run(async () =>
            {
                await Task.Delay(100);
                throw new Exception("Error");
#pragma warning disable CS0162 // Unreachable code detected
                return new PersonInfo();
#pragma warning restore CS0162 // Unreachable code detected
            }));



        var provider = Substitute.For<IPersonCrawlerProvider>();
        provider.GetAvailableCrawlerNames()
            .Returns(["Crawler1", "Crawler2"]);
        provider.GetFilteredCrawlerInstances(
                Arg.Any<string[]>())
            .Returns(call =>
            {
                var names = call.Arg<string[]>();
                List<IPersonCrawler> result = [crawler1, crawler2];
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


        var result = new PersonCrawler(provider);
        return result;
    }

}

