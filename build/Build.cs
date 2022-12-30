// https://github.com/dotnet/format/issues/1094
// https://docs.github.com/en/get-started/getting-started-with-git/configuring-git-to-handle-line-endings

using Basyc.Extensions.Nuke.Targets;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
///Nuke support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode  <summary>
[GitHubActions(
	"continuous",
	GitHubActionsImage.UbuntuLatest,
	OnPushBranches = new[] { "develop" },
	InvokedTargets = new[] { nameof(IBasycBuildContinuous.StaticCodeAnalysisAffected), nameof(IBasycBuildContinuous.UnitTestAffected) },
	EnableGitHubToken = false,
	FetchDepth = 0)]
[GitHubActions(
	"pullRequest",
	GitHubActionsImage.UbuntuLatest,
	OnPullRequestBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(IBasycBuildRelease.StaticCodeAnalysisAll), nameof(IBasycBuildRelease.UnitTestAll) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
[GitHubActions(
	"release",
	GitHubActionsImage.UbuntuLatest,
	OnPushBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(IBasycBuildRelease.StaticCodeAnalysisAll), nameof(IBasycBuildRelease.UnitTestAll), nameof(IBasycBuildRelease.NugetPushAll) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
internal class Build : NukeBuild, IBasycBuildAll
{
	//[Parameter] string NuGetSource => TryGetValue(() => NuGetSource);
	//[Parameter][Secret] string NuGetApiKey => TryGetValue(() => NuGetApiKey);
	//[Parameter][Secret] string NuGetApiPrivateKeyPfxBase64 => TryGetValue(() => NuGetApiPrivateKeyPfxBase64);
	//[Parameter][Secret] string NuGetApiCertPassword => TryGetValue(() => NuGetApiCertPassword);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	string IBasycBuildRelease.NugetSourceUrl => GitHubActions.Instance.GetNugetSourceUrl();

	string IBasycBuildRelease.NuGetApiKey => GitHubActions.Instance.Token;

	public static int Main()
	{
		IBasycBuildBase.BuildProjectName = "_build";
		IBasycBuildBase.UnitTestSuffix = ".UnitTests";
		return Execute<Build>(x => ((IBasycBuildAll)x).UnitTestAffected);
	}
}
