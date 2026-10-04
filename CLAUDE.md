# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

aemarcoCore is a set of .NET 9 NuGet libraries for crawling: people/model info and wallpapers. Three packages plus tests and a Nuke build:

- `aemarco.Crawler` (`aemarcoCrawler`) — shared core: HTML fetching/navigation (`HtmlHelper`, `PageUri`, `PageDocument`, `PageNode`), `CrawlerAttribute`, `SkipTestingAttribute`, `CountryService`, `CrawlerInfo`.
- `aemarco.Crawler.Person` (`aemarcoPersonCrawler`) — `IPersonCrawler` aggregating many site crawlers into a merged `PersonInfo`.
- `aemarco.Crawler.Wallpaper` (`aemarcoWallpaperCrawler`) — wallpaper site crawlers mapped to an internal `Category` enum.

## Commands

Build/test/pack go through Nuke (`build/`), which is also what CI (Azure Pipelines, `./build.cmd Publish`) runs:

```
./build.cmd Compile      # CountryUpdate -> Restore -> Compile
./build.cmd Tests        # + tests with coverage (trx/cobertura in build/output)
./build.cmd Pack         # nupkgs into build/output/drop
```

Plain dotnet works for day-to-day work (NUnit + FluentAssertions):

```
dotnet test Tests/aemarco.CrawlerTests
dotnet test Tests/aemarco.Crawler.PersonTests --filter "FullyQualifiedName~FreeonesTestsWithFoxyDi"
dotnet test --filter "Name=Crawler_Country"
```

Note: the Nuke `CountryUpdate` target **rewrites** `aemarco.Crawler/Resources/CountryRegionData.json` (merges CultureInfo, CountryData.Standard and hardcoded data) on every Nuke build, so expect that file to show up as modified after a build. Versioning is automatic via Nerdbank.GitVersioning (`version.json`, 9.0).

## Architecture

**Person crawler** (`aemarco.Crawler.Person`)
- Each site is a class in `Crawlers/` deriving `SiteCrawlerBase` (implements internal `ISiteCrawler`). It must carry `[Crawler("Friendly Name", priority)]` — `AddPersonCrawler()` discovers all non-abstract `ISiteCrawler` types by reflection, throws if the attribute is missing, and registers each as a **keyed transient** service by type name. File-name prefix (`12_Freeones`) mirrors the priority; lower value wins on merge.
- Subclasses implement `GetGirlUri(name)` and `HandlePersonEntry(page, token)` (and optionally `HandlePersonNameEntries`), filling the base-class `Result` via helpers (`UpdateName`, `UpdateCountry`, `UpdateMeasurements`, `UpdateSocial`, `UpdateProfilePictures`, ...).
- `PersonCrawler` runs all (optionally filtered via `AddPersonSiteFilter`) crawlers concurrently, collects failures into `PersonInfo.Errors` rather than throwing, then `PersonInfo.Merge`s results. `Crawlers/Obsolete/` holds dead sites kept for reference.

**Wallpaper crawler** — `WallpaperCrawlerBasis` per site in `Crawlers/`, each mapping site category names to `ContentCategory` (internal `Category` enum plus suggested adult-level range) via `GetContentCategory`.

**Country resolution** — `CountryService` (in core, backed by embedded `CountryRegionData.json`) matches whole words, longest name first; keep that invariant (a past bug matched "Oman" inside "Romania").

## Testing

- Person crawler tests hit the **live sites**. Per-site tests inherit `PersonInfoTestsBase<TCrawler>` (`Tests/aemarco.Crawler.PersonTests/_TestStuff`) and declare `Expected*` properties for one sample person; a null expectation just prints the actual value instead of asserting. `[SkipTesting]` on a crawler class makes its tests no-op. Failures are often site changes, not code bugs.
- Tests resolve crawlers via `IocHelper` using the real `AddPersonCrawler()` registration with `NullLogger`.
- `InternalsVisibleTo` for `<Project>Tests` is wired in `Directory.Build.props`, so tests can use internal types.
