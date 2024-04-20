//using System;
//using System.Linq;

using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Tools.ReportGenerator;
using Serilog;


//using Nuke.Common.CI;
//using Nuke.Common.Execution;
//using Nuke.Common.Tooling;
//using static Nuke.Common.EnvironmentInfo;
//using static Nuke.Common.IO.FileSystemTasks;
//using static Nuke.Common.IO.PathConstruction;

// ReSharper disable AllUnderscoreLocalParameterName

//https://nuke.build/

[AzurePipelines(
    AzurePipelinesImage.WindowsLatest,
    InvokedTargets = [
        nameof(Drop)
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



    //Tools
    AzurePipelines AzurePipelines => AzurePipelines.Instance;

    Target Info => _ => _
        .DependentFor(Clean, Restore, Compile, Tests, Pack, Drop)
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



    Target Tests => d => d
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testDir = TemporaryDirectory / "tests";
            testDir.CreateOrCleanDirectory();

            DotNetTasks.DotNetTest(t => t
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetDataCollector("XPlat Code Coverage")
                .AddLoggers("trx")
                .SetResultsDirectory(testDir));
            ReportGeneratorTasks.ReportGenerator(new ReportGeneratorSettings()
                .SetTargetDirectory(testDir)
                .SetReports($"{testDir}/**/coverage.cobertura.xml")
                .SetReportTypes(ReportTypes.Cobertura));

            //"C:\Program Files\dotnet\dotnet.exe"
            //test D:\a\1\s\Tests\aemarco.Crawler.PersonTests\aemarco.Crawler.PersonTests.csproj
            //--logger trx
            //--results-directory D:\a\_temp
            //--collect "Code coverage"

            //"C:\Program Files\dotnet\dotnet.exe"
            //test D:\a\1\s\aemarcoCore.sln
            //--configuration Release
            //--collect "XPlat Code Coverage"
            //--logger trx
            //--no-build
            //--no-restore
            //--results-directory D:\a\1\s\.nuke\temp\tests



            //AzurePipelines?.PublishTestResults(
            //    "Bob", 
            //    AzurePipelinesTestResultsType.NUnit,  
            //    [ testDir / "Cobertura.xml"],
            //    true, configuration: Configuration);

            Log.Information("{TestResultsDirectory}", AzurePipelines?.TestResultsDirectory);
            AzurePipelines?.PublishCodeCoverage(
                AzurePipelinesCodeCoverageToolType.Cobertura,
                testDir / "Cobertura.xml",
                AzurePipelines.TestResultsDirectory);
        });

    Target Pack => _ => _
        .DependsOn(Tests)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(x => x
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(TemporaryDirectory / "drop"));
        });

    Target Drop => _ => _
        .DependsOn(Pack)
        .Produces(TemporaryDirectory / "drop" / "*.nupkg");










    //Target Clean => _ => _cl
    //    .OnlyWhenDynamic(() => IsLocalBuild)
    //    .DependsOn(Info)
    //    .Executes(() =>
    //    {
    //        ArtifactsDirectory.DeleteDirectory();
    //        DotNetTasks.DotNetClean(s => s
    //            .SetConfiguration(Configuration));
    //SourceDirectory
    //    .GlobDirectories("**/bin", "**/obj")
    //    .ForEach(x =>
    //        x.DeleteDirectory());
    //TestsDirectory
    //    .GlobDirectories("**/bin", "**/obj")
    //    .ForEach(x =>
    //        x.DeleteDirectory());

    //OutputDirectory.CreateOrCleanDirectory();

    //    });


    //AbsolutePath SourceDirectory => RootDirectory / "src";
    //AbsolutePath TestsDirectory => RootDirectory / "tests";
    //AbsolutePath OutputDirectory => RootDirectory / "output";


}
