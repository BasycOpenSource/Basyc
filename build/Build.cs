using GlobExpressions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

/// Support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(NugetPush) })]
class Build : NukeBuild
{
    [GitRepository] readonly GitRepository Repository;
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;

    GitHubActions GitHubActions => GitHubActions.Instance;
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

    public static int Main() => Execute<Build>(x => x.NugetPush);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(_ => _
                .SetProject(Solution));
        });

    Target Restore => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .After(Restore)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .EnableNoRestore()
                .SetProjectFile(Solution));
        });

    Target UnitTest => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .EnableNoRestore()
                .CombineWith(
                    Solution.GetProjects("*.UnitTests"),
                    (settings, unitTestProject) => settings
                        .SetProjectFile(unitTestProject)));
        });

    Target NugetPush => _ => _
    .DependsOn(UnitTest)
    .Executes(() =>
    {
        DotNetPack(_ => _
            .EnableNoRestore()
            .EnableNoBuild()
            .SetProject(Solution)
            .SetOutputDirectory(OutputPackagesDirectory));

        var nugetPackages = OutputPackagesDirectory.GlobFiles("*.nupkg");

        //DotNetNuGetPush(_ => _
        //    .SetSource("https://nuget.pkg.github.com/BasycOpenSource/index.json")
        //    .SetApiKey(GitHubActions.Token)
        //    .CombineWith(nugetPackages, (_, nugetPackage) => _
        //        .SetTargetPath(nugetPackage)
        //        ));

        Assert.NotNullOrWhiteSpace(GitHubActions.Token);

        DotNetNuGetPush(_ => _
        .SetSource("https://nuget.pkg.github.com/BasycOpenSource/index.json")
        .SetApiKey(GitHubActions.Token)
        .SetTargetPath(nugetPackages.First()));

    });

}
