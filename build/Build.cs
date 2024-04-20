using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;
using Serilog;

// ReSharper disable AllUnderscoreLocalParameterName

//https://nuke.build/

[AzurePipelines(
    AzurePipelinesImage.WindowsLatest,
    AutoGenerate = false,
    InvokedTargets = [
        nameof(Publish)
    ],
    NonEntryTargets = [
        nameof(Info),
        nameof(Clean),
        nameof(Restore),
        nameof(Compile),
        nameof(Tests),
        nameof(Pack)
    ])]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository]
    readonly GitRepository Repository;

    [Solution]
    readonly Solution Solution;

    [CanBeNull]
    AzurePipelines AzurePipelines => AzurePipelines.Instance;


    readonly AbsolutePath TrxDir = RootDirectory / "build" / "output" / "trx";
    readonly AbsolutePath CobDir = RootDirectory / "build" / "output" / "cob";
    readonly AbsolutePath DropDir = RootDirectory / "build" / "output" / "drop";

    Target Info => _ => _
        .DependentFor(Clean, Restore, Compile, Tests, Pack, Publish)
        .Before(Clean, Restore)
        .Executes(() =>
        {
            Log.Information("Configuration = {Configuration}", Configuration);

            if (IsLocalBuild)
                return;
            Log.Information("RepoUrl = {RepoUrl}", Repository.HttpsUrl);
            Log.Information("Branch = {Branch}", Repository.Branch);
            Log.Information("Commit = {Commit}", Repository.Commit);
            Log.Information("Tags = {Value}", Repository.Tags);
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(x => x
                .SetProject(Solution));
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution));
        });

    Target Tests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            //alt
            //"C:\Program Files\dotnet\dotnet.exe"
            //test D:\a\1\s\Tests\aemarco.Crawler.PersonTests\aemarco.Crawler.PersonTests.csproj
            //--logger trx
            //--results-directory D:\a\_temp
            //--collect "Code coverage"

            //actual
            //"C:\Program Files\dotnet\dotnet.exe"
            //test D:\a\1\s\aemarcoCore.sln
            //--configuration Release
            //--collect "Code Coverage"
            //--logger trx
            //--no-build
            //--no-restore
            //--results-directory D:\a\1\s\build\output\trx

            TrxDir.CreateOrCleanDirectory();
            CobDir.CreateOrCleanDirectory();
            DotNetTasks.DotNetTest(x => x
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetDataCollector("XPlat Code Coverage")
                .AddLoggers("trx")
                .SetResultsDirectory(TrxDir));
            ReportGeneratorTasks.ReportGenerator(new ReportGeneratorSettings()
                .SetTargetDirectory(CobDir)
                .SetReports($"{TrxDir}/**/coverage.cobertura.xml")
                .SetReportTypes(ReportTypes.Cobertura));
        });

    Target Pack => _ => _
        .DependsOn(Tests)
        .Executes(() =>
        {
            DropDir.CreateOrCleanDirectory();
            DotNetTasks.DotNetPack(x => x
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(DropDir));
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Produces(DropDir / "*.nupkg")
        .Executes(() =>
        {

            //AzurePipelines?.PublishTestResults(
            //    "dotnet test all",
            //    AzurePipelinesTestResultsType.NUnit,
            //    Glob.Files(TrxDir, "*.trx"));



            //D:\a\1\TestResults
            //Log.Information("{TestResultsDirectory}", AzurePipelines?.TestResultsDirectory);
            //AzurePipelines?.PublishCodeCoverage(
            //    AzurePipelinesCodeCoverageToolType.Cobertura,
            //    testDir / "Cobertura.xml",
            //    AzurePipelines.TestResultsDirectory);
        });

}
