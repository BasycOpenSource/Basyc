using Basyc.Extensions.Nuke.Targets;
using Basyc.Extensions.Nuke.Targets.Nuget;
using Basyc.Extensions.Nuke.Tasks.CI;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
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
	new[] { nameof(IBasycBuildNugetAll.NugetReleaseAll) })]
class Build : NukeBuild, IBasycBuilds
{
	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	public Solution Solution = null!;

	Nuke.Common.ProjectModel.Solution IBasycBuildBase.Solution => Solution;

	string IBasycBuildBase.BuildProjectName => "_build";
	string IBasycBuildNugetAll.NugetSourceUrl => GitHubActions.Instance.GetNugetSourceUrl();
	string IBasycBuildNugetAll.NuGetApiKey => GitHubActions.Instance.Token;

	UnitTestSettings IBasycBuildBase.UnitTestSettings => UnitTestSettings.Create()
		.SetBranchMinimum(0)
		.SetSequenceMinimum(0)
		.Exclude(Solution.buildFolder._build);

	PullRequestSettings IBasycBuildBase.PullRequestSettings => PullRequestSettings.Create()
		.SetIsPullRequest(GitHubActions.Instance is not null && GitHubActions.Instance.IsPullRequest)
		.SetSourceBranch(GitHubActions.Instance?.GetPullRequestSourceBranch())
		.SetTargetBranch(GitHubActions.Instance?.GetPullRequestTargetBranch());

	// PullRequestSettings IBasycBuildBase.PullRequestSettings => PullRequestSettings.Create()
	// 	.SetIsPullRequest(true)
	// 	.SetSourceBranch("feature/GitOps")
	// 	.SetTargetBranch("develop");

	public static int Main()
	{
		return Execute<Build>(x => ((IBasycBuilds)x).UnitTestAll);
	}
}
