# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

aemarcoCore is a set of .NET 10 NuGet libraries for crawling: people/model info and wallpapers. Three packages plus tests and a Nuke build:

- `aemarco.Crawler` (`aemarcoCrawler`) — shared core: HTML fetching/navigation (`HtmlHelper`, `PageUri`, `PageDocument`, `PageNode`), `CrawlerAttribute`, `SkipTestingAttribute`, `CountryService`, `CrawlerInfo`.
- `aemarco.Crawler.Person` (`aemarcoPersonCrawler`) — `IPersonCrawler` aggregating many site crawlers into a merged `PersonInfo`.
- `aemarco.Crawler.Wallpaper` (`aemarcoWallpaperCrawler`) — wallpaper site crawlers mapped to an internal `Category` enum.

## Conventions

The general conventions (code style, test conventions, commit workflow, and the DO NOT rules, including the deploy stance) live in the aemarcoInternals `CLAUDE.md` and apply here as well: `../aemarcoCentral/aemarcoInternals/CLAUDE.md`. This file only describes what is specific to this repo; this repo's own `.editorconfig` is the style reference here.

## Commands

Build/test/pack go through Nuke (`build/`), which is also what CI (GitHub Actions, `.github/workflows/build.yml`, hand-written, runs `./build.cmd Publish` and uploads the `drop` and `test-results` artifacts) runs. On GitHub, test results show up as annotations and job summary (`GitHubActionsTestLogger`, referenced through `aemarco.TestBasics`), and the `Tests` target appends ReportGenerator's `SummaryGithub.md` coverage table to `$GITHUB_STEP_SUMMARY`. There is no `global.json`: the build uses the installed SDK, the workflow sets up `10.x`. The Nuke targets work on the whole solution; to verify a change, build or test only the project at hand (see below), as in the Internals file. Packages reach nuget.org outside this repo (the pipeline only produces the artifact), so the deploy stance of the Internals file applies to pushing and publishing here too:

```
./build.cmd Compile      # CountryUpdate -> Restore -> Compile
./build.cmd Tests        # + tests with coverage (trx/cobertura in build/output)
./build.cmd Pack         # nupkgs into build/output/drop
```

Plain dotnet works for day-to-day work (the tests here use NUnit and FluentAssertions, where Internals uses Shouldly; use what the file you edit already uses):

```
dotnet test Tests/aemarco.CrawlerTests
dotnet test Tests/aemarco.Crawler.PersonTests --filter "FullyQualifiedName~FreeonesTestsWithFoxyDi"
dotnet test --filter "Name=Crawler_Country"
```

Note: the Nuke `CountryUpdate` target **rewrites** `aemarco.Crawler/Resources/CountryRegionData.json` (merges CultureInfo, CountryData.Standard and hardcoded data) on every Nuke build, so expect that file to show up as modified after a build. Versioning is automatic via Nerdbank.GitVersioning (`version.json`, 10.0).

## Architecture

**Person crawler** (`aemarco.Crawler.Person`)
- Each site is a class in `Crawlers/` deriving `SiteCrawlerBase` (implements internal `ISiteCrawler`). It must carry `[Crawler("Friendly Name", priority)]` — `AddPersonCrawler()` discovers all non-abstract `ISiteCrawler` types by reflection, throws if the attribute is missing, and registers each as a **keyed transient** service by type name. File-name prefix (`12_Freeones`) mirrors the priority; lower value wins on merge.
- Subclasses implement `GetGirlUri(name)` and `HandlePersonEntry(page, token)` (and optionally `HandlePersonNameEntries`), filling the base-class `Result` via helpers (`UpdateName`, `UpdateCountry`, `UpdateMeasurements`, `UpdateSocial`, `UpdateProfilePictures`, ...).
- `PersonCrawler` runs all (optionally filtered via `AddPersonSiteFilter`) crawlers concurrently, collects failures into `PersonInfo.Errors` rather than throwing, then `PersonInfo.Merge`s results. `Crawlers/Obsolete/` holds dead sites kept for reference.

**Wallpaper crawler** — `WallpaperCrawlerBasis` per site in `Crawlers/`, each mapping site category names to `ContentCategory` (internal `Category` enum plus suggested adult-level range) via `GetContentCategory`. `AddWallCrawler()` registers every `WallpaperCrawlerBasis` subclass as a **keyed transient** by type name; the key list lives in its own `WallCrawlerRegistry` type, so it does not collide with the person registrations in a shared container. Crawler instances hold the state of one crawl, `WallCrawlerProvider` hands out fresh ones per `CrawlWallpapers`. Paging comes with `Start`, not the constructor (`FirstPageOnly` for sites that only ever crawl page 1). Each site × internal category is a crawl row in aemarcoInternals, so changing which crawlers are found or their friendly names adds or drops rows there.

**Country resolution** — `CountryService` (in core, backed by embedded `CountryRegionData.json`) matches whole words, longest name first; keep that invariant (a past bug matched "Oman" inside "Romania").

## Testing

- Person crawler tests hit the **live sites**. Per-site tests inherit `PersonInfoTestsBase<TCrawler>` (`Tests/aemarco.Crawler.PersonTests/_TestStuff`) and declare `Expected*` properties for one sample person; a null expectation just prints the actual value instead of asserting. `[SkipTesting]` on a crawler class makes its tests no-op. Failures are often site changes, not code bugs.
- Tests resolve crawlers via `IocHelper` using the real `AddPersonCrawler()` registration with `NullLogger`.
- `InternalsVisibleTo` for `<Project>Tests` is wired in `Directory.Build.props`, so tests can use internal types.
