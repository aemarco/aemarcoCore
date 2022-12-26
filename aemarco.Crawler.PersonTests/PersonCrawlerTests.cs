using aemarco.Crawler.Person;
using aemarco.Crawler.Person.Common;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.PersonTests;

public class PersonCrawlerTests
{

    [Test]
    public void GetAvailableCrawlers_DeliversCorrectly()
    {
        var crawlerNames = Assembly
            .GetAssembly(typeof(PersonCrawlerBase))!
            .GetTypes()
            .Where(x => x.IsAvailableCrawler())
            .Select(x => x.ToCrawlerInfo().FriendlyName)
            .ToList();

        var crawler = new PersonCrawler();
        var available = crawler.GetAvailableCrawlers().ToList();

        foreach (var crawlerName in crawlerNames)
        {
            available.Should().Contain(crawlerName);
        }
    }


    [Test]
    public async Task StartAsync_MergesResults()
    {
        var crawler = new PersonCrawler();
        var result = await crawler.StartAsync("Foxi Di", CancellationToken.None);

        if (result is null)
            throw new Exception("Did not get a PersonInfo");

        result.ProfilePictures.Count.Should().Be(8);
        result.Aliases.Count.Should().Be(28);
        result.Piercings.Should().Be("Navel");
    }


}