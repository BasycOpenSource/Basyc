using Nuke.Common;
using Nuke.Common.Git;

/// Support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode
class Build : NukeBuild
{


    [GitRepository] readonly GitRepository Repository;
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Before(Compile)
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .After(Restore)
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

}
