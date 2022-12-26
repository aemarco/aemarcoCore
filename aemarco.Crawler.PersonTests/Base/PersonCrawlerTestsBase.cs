using aemarco.Crawler.Person;
using aemarco.Crawler.Person.Common;
using aemarco.Crawler.Person.Model;
using FluentAssertions;
using Newtonsoft.Json;
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
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.PersonEntrySource,
                Formatting.Indented));
    }

    protected string ExpectedFirstName { get; set; }
    [Test]
    public void Crawler_Finds_FirstName()
    {
        if (Entry is null)
            return;

        Assert.AreEqual(ExpectedFirstName, Entry.FirstName);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.FirstName,
                Formatting.Indented));
    }

    protected string ExpectedLastName { get; set; }
    [Test]
    public void Crawler_Finds_LastName()
    {
        if (Entry is null)
            return;

        Assert.AreEqual(ExpectedLastName, Entry.LastName);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.LastName,
                Formatting.Indented));
    }

    protected Gender? ExpectedGender { get; set; }
    [Test]
    public void Crawler_Finds_Gender()
    {
        if (Entry is null)
            return;

        if (ExpectedGender is null)
        {
            NothingExpected(Entry.Gender);
            return;
        }

        Assert.AreEqual(ExpectedGender, Entry.Gender);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Gender,
                Formatting.Indented));
    }

    protected DateOnly? ExpectedBirthday { get; set; }
    [Test]
    public void Crawler_Finds_Birthday()
    {
        if (Entry is null)
            return;

        if (!ExpectedBirthday.HasValue)
        {
            NothingExpected(Entry.Birthday);
            return;
        }

        Assert.AreEqual(ExpectedBirthday!.Value, Entry.Birthday);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Birthday,
                Formatting.Indented));
    }

    protected DateOnly? ExpectedCareerStart { get; set; }
    [Test]
    public void Crawler_Finds_CareerStart()
    {
        if (Entry is null)
            return;

        if (!ExpectedCareerStart.HasValue)
        {
            NothingExpected(Entry.CareerStart);
            return;
        }

        Assert.AreEqual(ExpectedCareerStart.Value, Entry.CareerStart);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.CareerStart,
                Formatting.Indented));
    }

    protected string? ExpectedCountry { get; set; }
    [Test]
    public void Crawler_Finds_Country()
    {
        if (Entry is null)
            return;
        if (ExpectedCountry is null)
        {
            NothingExpected(Entry.Country);
            return;
        }
        Assert.AreEqual(ExpectedCountry, Entry.Country);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Country,
                Formatting.Indented));
    }

    protected string? ExpectedCity { get; set; }
    [Test]
    public void Crawler_Finds_Place()
    {
        if (Entry is null)
            return;
        if (ExpectedCity is null)
        {
            NothingExpected(Entry.City);
            return;
        }
        Assert.AreEqual(ExpectedCity, Entry.City);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.City,
                Formatting.Indented));
    }

    protected string? ExpectedProfession { get; set; }
    [Test]
    public void Crawler_Finds_Profession()
    {
        if (Entry is null)
            return;
        if (ExpectedProfession is null)
        {
            NothingExpected(Entry.Profession);
            return;
        }
        Assert.AreEqual(ExpectedProfession, Entry.Profession);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Profession,
                Formatting.Indented));
    }

    protected string? ExpectedEthnicity { get; set; }
    [Test]
    public void Crawler_Finds_Ethnicity()
    {
        if (Entry is null)
            return;
        if (ExpectedEthnicity is null)
        {
            NothingExpected(Entry.Ethnicity);
            return;
        }
        Assert.AreEqual(ExpectedEthnicity, Entry.Ethnicity);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Ethnicity,
                Formatting.Indented));
    }

    protected string? ExpectedHairColor { get; set; }
    [Test]
    public void Crawler_Finds_Hair()
    {
        if (Entry is null)
            return;
        if (ExpectedHairColor is null)
        {
            NothingExpected(Entry.HairColor);
            return;
        }
        Assert.AreEqual(ExpectedHairColor, Entry.HairColor);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.HairColor,
                Formatting.Indented));
    }


    protected string? ExpectedEyeColor { get; set; }
    [Test]
    public void Crawler_Finds_Eyes()
    {
        if (Entry is null)
            return;
        if (ExpectedEyeColor is null)
        {
            NothingExpected(Entry.EyeColor);
            return;
        }
        Assert.AreEqual(ExpectedEyeColor, Entry.EyeColor);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.EyeColor,
                Formatting.Indented));
    }

    protected int? ExpectedHeight { get; set; }
    [Test]
    public void Crawler_Finds_Height()
    {
        if (Entry is null)
            return;

        if (!ExpectedHeight.HasValue)
        {
            NothingExpected(Entry.Height);
            return;
        }
        Assert.AreEqual(ExpectedHeight!.Value, Entry.Height);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Height,
                Formatting.Indented));
    }

    protected int? ExpectedWeight { get; set; }
    [Test]
    public void Crawler_Finds_Weight()
    {
        if (Entry is null)
            return;
        if (!ExpectedWeight.HasValue)
        {
            NothingExpected(Entry.Weight);
            return;
        }
        Assert.AreEqual(ExpectedWeight!.Value, Entry.Weight);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Weight,
                Formatting.Indented));
    }

    protected string? ExpectedMeasurementDetails { get; set; }
    [Test]
    public void Crawler_Finds_Measurements()
    {
        if (Entry is null)
            return;
        if (ExpectedMeasurementDetails is null)
        {
            NothingExpected(Entry.MeasurementDetails);
            return;
        }
        Assert.AreEqual(ExpectedMeasurementDetails, Entry.MeasurementDetails?.ToString());

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.MeasurementDetails,
                Formatting.Indented));
    }


    protected string? ExpectedPiercings { get; set; }
    [Test]
    public void Crawler_Finds_Piercings()
    {
        if (Entry is null)
            return;

        if (ExpectedPiercings is null)
        {
            NothingExpected(Entry.Piercings);
            return;
        }
        Assert.AreEqual(ExpectedPiercings, Entry.Piercings);

        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Piercings,
                Formatting.Indented));
    }

    protected bool? ExpectedStillActive { get; set; }
    [Test]
    public void Crawler_Finds_StillActive()
    {
        if (Entry is null)
            return;

        Assert.AreEqual(ExpectedStillActive, Entry.StillActive);
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.StillActive,
                Formatting.Indented));
    }

    protected List<string> ExpectedProfilePictures { get; set; } = new();
    [Test]
    public void Crawler_Finds_Pictures()
    {
        if (Entry is null)
            return;

        if (ExpectedProfilePictures.Count == 0)
        {
            NothingExpected(Entry.ProfilePictures.Count == 0 ? null : Entry.ProfilePictures);
            return;
        }

        foreach (var url in ExpectedProfilePictures)
        {
            Entry.ProfilePictures.Should().Contain(x => x.Url == url);
        }
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.ProfilePictures,
                Formatting.Indented));
    }

    protected List<string> ExpectedAliases { get; set; } = new();
    [Test]
    public void Crawler_Finds_Aliases()
    {
        if (Entry is null)
            return;

        if (ExpectedAliases.Count == 0)
        {
            NothingExpected(Entry.Aliases.Count == 0 ? null : Entry.Aliases);
            return;
        }

        foreach (var al in ExpectedAliases)
        {
            Entry.Aliases.Should().Contain(al);
        }
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
                Entry.Aliases,
                Formatting.Indented));
    }


    private void NothingExpected(object? found)
    {
        TestContext.Out.WriteLine(
            JsonConvert.SerializeObject(
               new
               {
                   Expected = "Noting",
                   Found = found ?? "Nothing"
               },
               Formatting.Indented));
    }
}