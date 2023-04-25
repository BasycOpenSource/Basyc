using Basyc.Extensions.Nuke.Targets;
using Basyc.Extensions.Nuke.Targets.Nuget;
using Basyc.Extensions.Nuke.Tasks.CI;
using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Basyc.Extensions.Nuke.Tasks.Tools.GitFlow;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;

[BasycContinuousPipeline(
    CiProvider.GithubActions,
    PipelineOs.Linux,
    new[] { nameof(IBasycBuildCommonAffected.StaticCodeAnalysisAffected), nameof(IBasycBuildCommonAffected.UnitTestAffected) })]
[BasycPullRequestPipeline(
    CiProvider.GithubActions,
    PipelineOs.Linux,
    new[] { nameof(IBasycBuildCommonAll.StaticCodeAnalysisAll), nameof(IBasycBuildCommonAll.UnitTestAll) })]
[BasycReleasePipeline(
    CiProvider.GithubActions,
    PipelineOs.Linux,
    new[] { nameof(IBasycBuildNugetAll.NugetReleaseAll) },
    new[] { nameof(nugetApiKey) },
    new[] { nameof(nugetSource) })]
class Build : NukeBuild, IBasycBuilds
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    [Secret]
    readonly string nugetApiKey = null!;

    [Parameter("Nuget source url")]
    readonly string nugetSource = null!;

    [GitFlow] public GitFlow GitFlow = null!;

    [Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
    public Solution Solution = null!;

    Nuke.Common.ProjectModel.Solution IBasycBuildBase.Solution => Solution;

    string IBasycBuildBase.BuildProjectName => "_build";

    NugetSettings IBasycBuildNugetAll.NugetSettings => NugetSettings.Create()
        .SetSourceUrl(nugetSource)
        .SetSourceApiKey(nugetApiKey);

    UnitTestSettings IBasycBuildBase.UnitTestSettings => UnitTestSettings.Create()
        .SetPublishResults(GitFlow.Branch is GitFlowBranch.Develop or GitFlowBranch.Main)
        .SetBranchMinimum(0)
        .SetSequenceMinimum(0)
        .Exclude(Solution.buildFolder._build);

    PullRequestSettings IBasycBuildBase.PullRequestSettings => PullRequestSettings.Create()
        .SetIsPullRequest(GitHubActions.Instance.IsPullRequest())
        .SetSourceBranch(GitHubActions.Instance?.GetPullRequestSourceBranch())
        .SetTargetBranch(GitHubActions.Instance?.GetPullRequestTargetBranch());

    public static int Main() => Execute<Build>(x => ((IBasycBuilds)x).StaticCodeAnalysisAffected);
}
