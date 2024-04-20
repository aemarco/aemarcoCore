//using System;
//using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
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
    InvokedTargets = [nameof(Pack)])]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository]
    readonly GitRepository Repository;




    [Solution(GenerateProjects = true)]
    readonly Solution Solution;
    Project Crawler => Solution.aemarco_Crawler;
    Project PersonCrawler => Solution.aemarco_Crawler_Person;
    Project WallpaperCrawler => Solution.aemarco_Crawler_Wallpaper;



    //Tools
    AzurePipelines AzurePipelines => AzurePipelines.Instance;


    Target Hosting => _ => _
        .Executes(() =>
        {
            Log.Information("Host {Host}", Host);
        });

    Target Props => _ => _
        .DependsOn(Hosting)
        .Executes(() =>
        {
            Log.Information("IsLocalBuild {IsLocalBuild}", IsLocalBuild);
            Log.Information("IsServerBuild {IsServerBuild}", IsServerBuild);
            Log.Information("RootDirectory {RootDirectory}", RootDirectory);
            Log.Information("TemporaryDirectory {TemporaryDirectory}", TemporaryDirectory);
        });

    Target Code => _ => _
        .DependsOn(Props)
        .Executes(() =>
        {
            Log.Information("Solution path = {Value}", Solution);
            Log.Information("Solution directory = {Value}", Solution.Directory);
            //Log.Information("FullSemVer = {Value}", GitVersion.FullSemVer);

            Log.Information("Commit = {Value}", Repository.Commit);
            Log.Information("Branch = {Value}", Repository.Branch);
            Log.Information("Tags = {Value}", Repository.Tags);

            Log.Information("main branch = {Value}", Repository.IsOnMainBranch());
            Log.Information("main/master branch = {Value}", Repository.IsOnMainOrMasterBranch());
            Log.Information("release/* branch = {Value}", Repository.IsOnReleaseBranch());
            Log.Information("hotfix/* branch = {Value}", Repository.IsOnHotfixBranch());

            Log.Information("Https URL = {Value}", Repository.HttpsUrl);
            Log.Information("SSH URL = {Value}", Repository.SshUrl);
        });

    Target Info => _ => _
        .DependsOn(Code)
        .Executes(() =>
        {
            //Log.Information("ArtifactsDirectory {ArtifactsDirectory}", ArtifactsDirectory);
            //Log.Information("TestResultsDirectory {TestResultsDirectory}", TestResultsDirectory);
            //Log.Information("BuildArtifactsDirectory {BuildArtifactsDirectory}", BuildArtifactsDirectory);

            if (IsLocalBuild)
            {
                Log.Information("Running locally - skipping repo info");
                return;
            }

            Log.Information("Branch = {Branch}", AzurePipelines.SourceBranch);
            Log.Information("Commit = {Commit}", AzurePipelines.SourceVersion);
        });


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


    Target Clean => _ => _
        .Before(Restore)
        .DependsOn(Info)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(x => x
                .SetProject(Solution));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
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

            //DotNetTasks.DotNetBuild(s => s
            //.SetAssemblyVersion(GitVersion.AssemblySemVer)
            //.SetFileVersion(GitVersion.AssemblySemFileVer)
            //.SetInformationalVersion(GitVersion.InformationalVersion)
            //.SetConfiguration(Configuration));
        });

    Target UnitTest => d => d
        .DependsOn(Compile)
        .Executes(() =>
        {
            //DotNetTasks.DotNetTest(t => t
            //    .SetProjectFile(Solution)
            //    .SetConfiguration(Configuration));

        });


    Target Pack => _ => _
        .DependsOn(UnitTest)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(x => x
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetProject(Solution));
        });

}
