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
///   - Microsoft VSCode           https://nuke.build/vscode 

[GitHubActions(
	"continuous",
	GitHubActionsImage.WindowsLatest,
	OnPushBranches = new[] { "develop" },
	InvokedTargets = new[] { nameof(StaticCodeAnalysis), nameof(UnitTest) },
	EnableGitHubToken = true,
	FetchDepth = 0,
	CacheKeyFiles = new string[] { })]
[GitHubActions(
	"release",
	GitHubActionsImage.UbuntuLatest,
	OnPullRequestBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(StaticCodeAnalysis), nameof(UnitTest), nameof(NugetPush) },
	EnableGitHubToken = true,
	FetchDepth = 0)]
internal class Build : NukeBuild
{
	[GitRepository] private readonly GitRepository? Repository;
	[Solution(GenerateProjects = true)] private readonly Solution? Solution;
	[GitVersion] private readonly GitVersion? GitVersion;

	private GitHubActions GitHubActions => GitHubActions.Instance;

	private AbsolutePath OutputDirectory => RootDirectory / "output";

	private AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	public static int Main()
	{
		//ProjectModelTasks.Initialize(); //https://github.com/nuke-build/nuke/issues/844
		return Execute<Build>(x => x.StaticCodeAnalysis);
	}

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	private Target StaticCodeAnalysis => _ => _
		.Before(Compile)
		.Executes(() =>
		{
			if (GitHubActions is not null && GitHubActions.IsPullRequest)
			{
				string branchToComapre = Repository.IsOnDevelopBranch() ? "main" : Repository.IsOnMainBranch() ? throw new NotImplementedException() : "develop";
				var gitChanges = GitGetChangeReport(Repository!.LocalDirectory, branchToComapre);
				DotnetFormatVerifyNoChanges(gitChanges);
			}
			else
			{
				string branchToComapre = Repository.IsOnDevelopBranch() ? "main" : Repository.IsOnMainBranch() ? throw new NotImplementedException() : "develop";
				var gitChanges = GitGetChangeReport(Repository!.LocalDirectory, branchToComapre);
				DotnetFormatVerifyNoChanges(gitChanges);
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