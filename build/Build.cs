using GlobExpressions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static _build.DotNetTasks;
using static _build.GitTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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
	InvokedTargets = new[] { nameof(StaticCodeAnalysis), nameof(UnitTest) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
[GitHubActions(
	"release",
	GitHubActionsImage.UbuntuLatest,
	OnPullRequestBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(StaticCodeAnalysis), nameof(UnitTest), nameof(NugetPush) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
internal class Build : NukeBuild
{
	[Solution(GenerateProjects = true)] private readonly Solution? Solution;
	[GitRepository] private readonly GitRepository? Repository;
	[GitVersion] private readonly GitVersion? GitVersion;
	//[GitCompareReport] private readonly GitCompareReport? GitCompareReport;

	private GitHubActions GitHubActions => GitHubActions.Instance;
	private AbsolutePath OutputDirectory => RootDirectory / "output";
	private AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	public static int Main()
	{
		return Execute<Build>(x => x.StaticCodeAnalysis);
	}

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	private Target StaticCodeAnalysis => _ => _
		.Before(Compile)
		.Executes(() =>
		{
			var GitCompareReport = GitGetCompareReport(Repository!.LocalDirectory);
			if (GitCompareReport!.CouldCompare)
			{
				DotnetFormatVerifyNoChanges(GitCompareReport!);
			}
			else
			{
				DotnetFormatVerifyNoChanges(Solution!.Path);
			}
		});

	private Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			DotNetClean(_ => _
				.SetProject(Solution));
		});

	private Target Restore => _ => _
		.Before(Compile)
		.Executes(() =>
		{
			DotNetRestore(_ => _
				.SetProjectFile(Solution));
		});

	private Target Compile => _ => _
		.After(Restore)
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(_ => _
				.EnableNoRestore()
				.SetProjectFile(Solution));
		});

	private Target UnitTest => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			var unitTestProjects = Solution!.GetProjects("*.UnitTests");
			DotNetTest(_ => _
				.EnableNoRestore()
				.CombineWith(unitTestProjects,
					(settings, unitTestProject) => settings
						.SetProjectFile(unitTestProject)),
						degreeOfParallelism: 5);
		});

	private Target NugetPush => _ => _
		.DependsOn(UnitTest)
		.Executes(() =>
		{
			DotNetPack(_ => _
				.EnableNoRestore()
				.SetVersion(GitVersion!.NuGetVersionV2)
				.EnableNoBuild()
				.SetProject(Solution)
				.SetOutputDirectory(OutputPackagesDirectory));

			var nugetPackages = OutputPackagesDirectory.GlobFiles("*.nupkg");

			DotNetNuGetPush(_ => _
				.SetSource("https://nuget.pkg.github.com/BasycOpenSource/index.json")
				.SetApiKey(GitHubActions.Token)
				.CombineWith(nugetPackages, (_, nugetPackage) => _
					.SetTargetPath(nugetPackage)));
		});

}
