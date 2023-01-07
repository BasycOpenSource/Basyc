// https://github.com/dotnet/format/issues/1094
// https://docs.github.com/en/get-started/getting-started-with-git/configuring-git-to-handle-line-endings

using Basyc.Extensions.Nuke.Targets;
using Basyc.Extensions.Nuke.Targets.Nuget;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
///Nuke support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode 
[GitHubActions(
	"continuous",
	GitHubActionsImage.UbuntuLatest,
	OnPushBranches = new[] { "feature/*", "release/*", "hotfix/*" },
	InvokedTargets = new[] { nameof(IBasycBuildCommonAffected.StaticCodeAnalysisAffected), nameof(IBasycBuildCommonAffected.UnitTestAffected) },
	EnableGitHubToken = false,
	FetchDepth = 0)]
[GitHubActions(
	"pullRequest",
	GitHubActionsImage.UbuntuLatest,
	OnPullRequestBranches = new[] { "develop", "main" },
	InvokedTargets = new[] { nameof(IBasycBuildCommonAll.StaticCodeAnalysisAll), nameof(IBasycBuildCommonAll.UnitTestAll) },
	EnableGitHubToken = false,
	FetchDepth = 0)]
[GitHubActions(
	"release",
	GitHubActionsImage.UbuntuLatest,
	OnPushBranches = new[] { "develop", "main" },
	InvokedTargets = new[] { nameof(IBasycBuilds.NugetReleaseAll) },
	EnableGitHubToken = true,
	FetchDepth = 0)]

internal class Build : NukeBuild, IBasycBuilds
{

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	string IBasycBuildBase.BuildProjectName => "_build";
	string IBasycBuildBase.UnitTestSuffix => ".UnitTests";
	bool IBasycBuildBase.IsPullRequest => GitHubActions.Instance is not null && GitHubActions.Instance.IsPullRequest;
	string IBasycBuildBase.PullRequestSourceBranch => GitHubActions.Instance.GetPullRequestSourceBranch();
	string IBasycBuildBase.PullRequestTargetBranch => GitHubActions.Instance.GetPullRequestTargetBranch();
	string IBasycBuildNugetAll.NugetSourceUrl => GitHubActions.Instance.GetNugetSourceUrl();
	string IBasycBuildNugetAll.NuGetApiKey => GitHubActions.Instance.Token;

	double IBasycBuildCommonAffected.MinSequenceCoverage => 50;
	double IBasycBuildCommonAffected.MinBranchCoverage => 50;

	public static int Main()
	{
		return Execute<Build>(x => ((IBasycBuilds)x).UnitTestAffected);
	}
}
