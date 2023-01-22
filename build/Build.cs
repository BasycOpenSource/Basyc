using Basyc.Extensions.Nuke.Targets;
using Basyc.Extensions.Nuke.Targets.Nuget;
using Basyc.Extensions.Nuke.Tasks.CI;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;

#pragma warning disable CS8618
[BasycContinuousPipeline(
	CiProviders.GithubActions,
	HostOs.Linux,
	new[] { nameof(IBasycBuildCommonAffected.StaticCodeAnalysisAffected), nameof(IBasycBuildCommonAffected.UnitTestAffected) })]
[BasycPullRequestPipeline(
	CiProviders.GithubActions,
	HostOs.Linux,
	new[] { nameof(IBasycBuildCommonAll.StaticCodeAnalysisAll), nameof(IBasycBuildCommonAll.UnitTestAll) })]
[BasycReleasePipeline(
	CiProviders.GithubActions,
	HostOs.Linux,
	new[] { nameof(IBasycBuilds.NugetReleaseAll) })]
internal class Build : NukeBuild, IBasycBuilds
{
	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	public Solution Solution;

	Nuke.Common.ProjectModel.Solution IBasycBuildBase.Solution => Solution;
	string IBasycBuildBase.BuildProjectName => "_build";
	bool IBasycBuildBase.IsPullRequest => GitHubActions.Instance is not null && GitHubActions.Instance.IsPullRequest;
	string IBasycBuildBase.PullRequestSourceBranch => GitHubActions.Instance.GetPullRequestSourceBranch();
	string IBasycBuildBase.PullRequestTargetBranch => GitHubActions.Instance.GetPullRequestTargetBranch();
	string IBasycBuildNugetAll.NugetSourceUrl => GitHubActions.Instance.GetNugetSourceUrl();
	string IBasycBuildNugetAll.NuGetApiKey => GitHubActions.Instance.Token;

	UnitTestSettings IBasycBuildBase.UnitTestSettings => UnitTestSettings.Create()
		.SetBranchMinimum(50)
		.SetSequenceMinimum(50)
		.Exclude(Solution.buildFolder._build);

	public static int Main()
	{
		//comment1
		return Execute<Build>(x => ((IBasycBuilds)x).UnitTestAffected);
	}
}
