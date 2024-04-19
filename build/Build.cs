//using System;
//using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
//using Nuke.Common.CI;
//using Nuke.Common.Execution;
//using Nuke.Common.IO;
//using Nuke.Common.ProjectModel;
//using Nuke.Common.Tooling;
//using Nuke.Common.Utilities.Collections;
//using static Nuke.Common.EnvironmentInfo;
//using static Nuke.Common.IO.FileSystemTasks;
//using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

//https://nuke.build/

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.UnitTest);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    readonly Solution Solution;

    //Tools
    //[GitVersion]
    //readonly GitVersion GitVersion;

    //AzurePipelines AzurePipelines => AzurePipelines.Instance;

    Target Info => _ => _
        .Executes(() =>
        {
            Log.Information("Host {Host}", Host);
            Log.Information("IsLocalBuild {IsLocalBuild}", IsLocalBuild);
            Log.Information("IsServerBuild {IsServerBuild}", IsServerBuild);

            Log.Information("RootDirectory {RootDirectory}", RootDirectory);
            Log.Information("TemporaryDirectory {TemporaryDirectory}", TemporaryDirectory);

            Log.Information("BuildAssemblyFile {BuildAssemblyFile}", BuildAssemblyFile);
            Log.Information("BuildAssemblyDirectory {BuildAssemblyDirectory}", BuildAssemblyDirectory);
            Log.Information("BuildProjectFile {BuildProjectFile}", BuildProjectFile);
            Log.Information("BuildProjectDirectory {BuildProjectDirectory}", BuildProjectDirectory);

            //Log.Information("ArtifactsDirectory {ArtifactsDirectory}", ArtifactsDirectory);
            //Log.Information("TestResultsDirectory {TestResultsDirectory}", TestResultsDirectory);
            //Log.Information("BuildArtifactsDirectory {BuildArtifactsDirectory}", BuildArtifactsDirectory);



            Log.Information("Solution path = {Value}", Solution);
            Log.Information("Solution directory = {Value}", Solution.Directory);
            //Log.Information("FullSemVer = {Value}", GitVersion.FullSemVer);


            if (IsLocalBuild)
            {
                Log.Information("Running locally - skipping repo info");
                return;
            }

            //Log.Information("Branch = {Branch}", AzurePipelines.SourceBranch);
            //Log.Information("Commit = {Commit}", AzurePipelines.SourceVersion);
        });


    //Target Clean => _ => _cl
    //    .OnlyWhenDynamic(() => IsLocalBuild)
    //    .DependsOn(Info)
    //    .Executes(() =>
    //    {
    //        ArtifactsDirectory.DeleteDirectory();
    //        DotNetTasks.DotNetClean(s => s
    //            .SetConfiguration(Configuration));
    //    });


    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(x =>
                    x.DeleteDirectory());
            TestsDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(x =>
                    x.DeleteDirectory());

            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());

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
            //.SetProjectFile(Solution)
            //.SetConfiguration(Configuration)
            //.SetResultsDirectory(TestResultsDirectory));

        });

}
