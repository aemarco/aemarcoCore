using aemarco.TestBasics;
using NSubstitute;
using System.Threading;

namespace aemarco.Crawler.PersonTests;

internal class PersonCrawlerTests
{

    [Test]
    public void AvailableCrawlers_ProvidesNames()
    {
        var sut = GetPersonCrawler();

        var result = sut.AvailableCrawlers
            .ToArray();

        result.Should().Equal("Crawler1", "Crawler2");

        TestHelper.PrintPassed(result);
    }

    [Test]
    public async Task AddPersonSiteFilter_Filters()
    {
        var sut = GetPersonCrawler();

        sut.AddPersonSiteFilter("Crawler1");

        //we check indirectly if StartAsync uses the filter
        var result = await sut.CrawlPerson("foxi", "di");
        result.CrawlerInfos.Should().HaveCount(1);
        result.CrawlerInfos[0].FriendlyName.Should().Be("Crawler1");

        TestHelper.PrintPassed(result.CrawlerInfos);
    }

    [Test]
    public async Task CrawlPersonNames_Works()
    {
        //we ignore logging and errors here
        //we focus on deduplication and ordering

        var sut = GetPersonCrawler(true);

        var result = await sut.CrawlPersonNames(CancellationToken.None);

        result.Should().Equal(
            new PersonNameInfo("Ariel", "Rebel"),
            new PersonNameInfo("Foxi", "Di"),
            new PersonNameInfo("Piper", "Perri"));

        TestHelper.PrintPassed(result);
    }

    [Test]
    public async Task CrawlPerson_ShouldTitleName()
    {
        var sut = GetPersonCrawler();
        sut.AddPersonSiteFilter("Nope");

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(0);
        result.FirstName.Should().Be("Foxi");
        result.LastName.Should().Be("Di");

        TestHelper.PrintPassed(result);
    }

    [Test]
    public async Task CrawlPerson_ShouldUseCrawlers()
    {
        var sut = GetPersonCrawler();

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        result.CrawlerInfos.Select(x => x.FriendlyName).Should().BeEquivalentTo("Crawler1", "Crawler2");

        TestHelper.PrintPassed(result.CrawlerInfos);
    }

    [Test]
    public async Task CrawlPerson_ShouldHandleErrors()
    {
        var sut = GetPersonCrawler(true);

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Error");

        TestHelper.PrintPassed(result.Errors);
    }

    [Test]
    public async Task CrawlPerson_ShouldMergeResult()
    {
        var sut = GetPersonCrawler();

        var result = await sut.CrawlPerson("foxi", "di");

        result.CrawlerInfos.Should().HaveCount(2);
        //order check as well
        result.CrawlerInfos[0].FriendlyName.Should().Be("Crawler2");
        result.CrawlerInfos[1].FriendlyName.Should().Be("Crawler1");

        TestHelper.PrintPassed(result.CrawlerInfos);
    }

    //mock
    private static PersonCrawler GetPersonCrawler(bool withError = false)
    {
        var crawler1 = Substitute.For<IPersonCrawler>();
        crawler1.GetCrawlerInfo()
            .Returns(new CrawlerInfo("Crawler1", 2));
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
                new PersonNameInfo("Foxi", "Di"),
                new PersonNameInfo("Ariel", "Rebel")
            ]);


        var crawler2 = Substitute.For<IPersonCrawler>();
        crawler2.GetCrawlerInfo()
            .Returns(new CrawlerInfo("Crawler2", 1));
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
                new PersonNameInfo("Piper", "Perri"),
                new PersonNameInfo("Ariel", "Rebel")
            ]);

        var crawler3 = Substitute.For<IPersonCrawler>();
        crawler2.GetCrawlerInfo()
            .Returns(new CrawlerInfo("Crawler3", 99));
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
                return Array.Empty<PersonNameInfo>();
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

