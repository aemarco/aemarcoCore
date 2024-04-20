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
    InvokedTargets = [nameof(Info), nameof(Publish)])]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository]
    readonly GitRepository Repository;

    [Solution]
    readonly Solution Solution;

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
        .Produces(DropDir / "*.nupkg");

}
