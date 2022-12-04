﻿using aemarco.Crawler.Person;
using aemarco.Crawler.Person.Common;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.PersonTests;

public class PersonCrawlerTests
{

    [Test]
    public void GetAvailableCrawlers_DeliversCorrectly()
    {
        var types = PersonCrawler.GetAvailableCrawlerTypes();
        var infos = types.Select(x => x.ToCrawlerInfo()).ToList();

        var crawler = new PersonCrawler();
        var available = crawler.GetAvailableCrawlers().ToList();


        foreach (var info in infos)
        {
            available.Should().Contain(info.FriendlyName);
        }
    }

    [Test]
    public async Task StartAsync_DoesNotCrawlDisabledCrawlers()
    {
        var crawler = new PersonCrawler();
        await crawler.StartAsync("Foxi Di", CancellationToken.None);
    }

    [Test]
    public async Task StartAsync_MergesResults()
    {
        var crawler = new PersonCrawler();
        var result = await crawler.StartAsync("Foxi Di", CancellationToken.None);

        if (result is null)
            throw new Exception("Did not get a PersonInfo");

        result.ProfilePictures.Count.Should().Be(7);
        result.Aliases.Count.Should().Be(18);
        result.Piercings.Should().Be("Navel");

    }


}