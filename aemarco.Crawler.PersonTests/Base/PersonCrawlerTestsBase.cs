using aemarco.Crawler.Person;
using aemarco.Crawler.Person.Common;
using aemarco.Crawler.Person.Model;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace aemarco.Crawler.PersonTests.Base;

internal abstract class PersonCrawlerTestsBase<T>
{
    private readonly string _nameToCrawl;

    protected PersonCrawlerTestsBase(string nameToCrawl)
    {
        ExpectedFirstName = nameToCrawl.Split(' ')[0];
        ExpectedLastName = nameToCrawl.Split(' ')[1];
        _nameToCrawl = nameToCrawl;
    }

    [OneTimeSetUp]
    public void Init()
    {
        var type = PersonCrawler
            .GetAvailableCrawlerTypes()
            .FirstOrDefault(x => x.FullName == typeof(T).FullName);

        if (type is null)
            throw new Exception($"Type {typeof(T).FullName} not available");

        Info = type.ToCrawlerInfo();

        var crawler = new PersonCrawler();
        crawler.AddPersonSiteFilter(Info.FriendlyName);
        Entry = crawler.StartAsync(_nameToCrawl).GetAwaiter().GetResult();
    }

    protected PersonCrawlerAttribute Info { get; private set; } = null!;
    protected PersonInfo? Entry { get; private set; }



    [Test]
    public void Entry_Matches_PersonSite()
    {
        if (Entry is null)
            return;

        Assert.AreEqual(Info.FriendlyName, Entry.PersonEntrySource);
    }


    protected string ExpectedFirstName { get; set; }
    [Test]
    public void Crawler_Finds_FirstName()
    {
        if (Entry is null) return;
        Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
    }

    protected string ExpectedLastName { get; set; }
    [Test]
    public void Crawler_Finds_LastName()
    {
        if (Entry is null)
            return;
        Assert.AreEqual(ExpectedLastName, Entry.LastName);
    }

    protected DateTime? ExpectedBirthday { get; set; }
    [Test]
    public void Crawler_Finds_Birthday()
    {
        if (Entry is null)
            return;
        if (!ExpectedBirthday.HasValue)
            return;
        Assert.AreEqual(ExpectedBirthday!.Value, Entry.Birthday);
    }

    protected DateTime? ExpectedCareerStart { get; set; }
    [Test]
    public void Crawler_Finds_CareerStart()
    {
        if (Entry is null)
            return;
        if (!ExpectedCareerStart.HasValue)
            return;
        Assert.AreEqual(ExpectedCareerStart!.Value, Entry.CareerStart);
    }



    protected string? ExpectedCountry { get; set; }
    [Test]
    public void Crawler_Finds_Country()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedCountry))
            return;
        Assert.AreEqual(ExpectedCountry, Entry.Country);
    }

    protected string? ExpectedPlace { get; set; }
    [Test]
    public void Crawler_Finds_Place()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedPlace))
            return;
        Assert.AreEqual(ExpectedPlace, Entry.City);
    }

    protected string? ExpectedProfession { get; set; }
    [Test]
    public void Crawler_Finds_Profession()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedProfession))
            return;
        Assert.AreEqual(ExpectedProfession, Entry.Profession);
    }
    protected string? ExpectedEthnicity { get; set; }
    [Test]
    public void Crawler_Finds_Ethnicity()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedEthnicity))
            return;
        Assert.AreEqual(ExpectedEthnicity, Entry.Ethnicity);
    }

    protected string? ExpectedHairColor { get; set; }
    [Test]
    public void Crawler_Finds_Hair()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedHairColor))
            return;
        Assert.AreEqual(ExpectedHairColor, Entry.HairColor);
    }


    protected string? ExpectedEyeColor { get; set; }
    [Test]
    public void Crawler_Finds_Eyes()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedEyeColor))
            return;
        Assert.AreEqual(ExpectedEyeColor, Entry.EyeColor);
    }

    protected int? ExpectedHeight { get; set; }
    [Test]
    public void Crawler_Finds_Height()
    {
        if (Entry is null)
            return;
        if (!ExpectedHeight.HasValue)
            return;
        Assert.AreEqual(ExpectedHeight!.Value, Entry.Height);
    }

    protected int? ExpectedWeight { get; set; }
    [Test]
    public void Crawler_Finds_Weight()
    {
        if (Entry is null)
            return;
        if (!ExpectedWeight.HasValue)
            return;
        Assert.AreEqual(ExpectedWeight!.Value, Entry.Weight);
    }

    protected string? ExpectedMeasurements { get; set; }
    [Test]
    public void Crawler_Finds_Measurements()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedMeasurements))
            return;
        Assert.AreEqual(ExpectedMeasurements, Entry.Measurements);
    }

    protected string? ExpectedCupsize { get; set; }
    [Test]
    public void Crawler_Finds_CupSize()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedCupsize))
            return;
        Assert.AreEqual(ExpectedCupsize, Entry.CupSize);
    }

    protected string? ExpectedPiercings { get; set; }
    [Test]
    public void Crawler_Finds_Piercings()
    {
        if (Entry is null)
            return;
        if (string.IsNullOrWhiteSpace(ExpectedPiercings))
            return;
        Assert.AreEqual(ExpectedPiercings, Entry.Piercings);
    }

    protected bool? ExpectedStillActive { get; set; }
    [Test]
    public void Crawler_Finds_StillActive()
    {
        if (Entry is null)
            return;
        Assert.AreEqual(ExpectedStillActive, Entry.StillActive);
    }

    protected List<string> ExpectedProfilePictures { get; set; } = new();
    [Test]
    public void Crawler_Finds_Pictures()
    {
        if (Entry is null)
            return;
        foreach (var url in ExpectedProfilePictures)
        {
            Entry.ProfilePictures.Should().Contain(x => x.Url == url);
        }
    }

    protected List<string> ExpectedAliases { get; set; } = new();
    [Test]
    public void Crawler_Finds_Aliases()
    {
        if (Entry is null)
            return;
        foreach (var al in ExpectedAliases)
        {
            Assert.IsTrue(Entry.Aliases.Contains(al));
        }
    }


}