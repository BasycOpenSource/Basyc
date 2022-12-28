using Basyc.Extensions.Nuke.Targets;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;

///Nuke support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode  <summary>
// https://github.com/dotnet/format/issues/1094
// https://docs.github.com/en/get-started/getting-started-with-git/configuring-git-to-handle-line-endings

[GitHubActions(
	"continuous",
	GitHubActionsImage.UbuntuLatest,
	OnPushBranches = new[] { "develop" },
	InvokedTargets = new[] { nameof(IBasycBuildAffected.StaticCodeAnalysisAffected), nameof(IBasycBuildAffected.UnitTestAffected) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
[GitHubActions(
	"release",
	GitHubActionsImage.UbuntuLatest,
	OnPullRequestBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(IBasycBuildAffected.StaticCodeAnalysisAll), nameof(IBasycBuildAffected.UnitTestAll), nameof(IBasycBuildAffected.NugetPushAll) },
	ImportSecrets = new[] { nameof(NuGetApiKey) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
internal class Build : NukeBuild, IBasycBuildAffected
{
	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Parameter][Secret] private readonly string? NuGetApiKey;
	[Parameter] private readonly string? NuGetSource;

	private GitHubActions GitHubActions => GitHubActions.Instance;

	public static int Main()
	{
		IBasycBuildAffected.BuildProjectName = "_build";
		IBasycBuildAffected.UnitTestSuffix = ".UnitTests";
		return Execute<Build>(x => ((IBasycBuildAffected)x).NugetPushAll);
	}
}
