using GlobExpressions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using Tasks.Git.Diff;
using static _build.DotNetTasks;
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
internal class Build2 : NukeBuild
{
	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution(GenerateProjects = true)] private readonly Solution? Solution;
	[GitRepository] private readonly GitRepository? Repository;
	[GitVersion] private readonly GitVersion? GitVersion;
	[GitCompareReport] private readonly GitCompareReport? GitCompareReport;

	private GitHubActions GitHubActions => GitHubActions.Instance;
	private AbsolutePath OutputDirectory => RootDirectory / "output";
	private AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	public static int Main2()
	{
		return Execute<Build2>(x => x.UnitTest);
	}

	private Target StaticCodeAnalysis => _ => _
		.Before(Compile)
		.Executes(() =>
		{
			if (GitCompareReport!.CouldCompare)
			{
				DotnetFormatVerifyNoChanges(GitCompareReport!);
			}
			else
			{
				Log.Error($"Git compare report unavailable. Running dotnet format for all files.");
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
			if (GitCompareReport!.CouldCompare)
			{
				var changedProjects = GitCompareReport.ChangedSolutions
				.SelectMany(x => x.ChangedProjects)
				.Select(x => x.ProjectFullPath);

				DotNetBuild(_ => _
					.EnableNoRestore()
					.CombineWith(changedProjects, (_, changedProject) => _
					.SetProjectFile(changedProject)));
			}
			else
			{
				Log.Error($"Git compare report unavailable. Building whole solution.");
				DotNetBuild(_ => _
					.EnableNoRestore()
					.SetProjectFile(Solution));
			}
		});

	private Target UnitTest => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{

			if (GitCompareReport!.CouldCompare)
			{
				var unitTestProjectsPaths = GitCompareReport.ChangedSolutions
					.SelectMany(x => x.ChangedProjects)
					.Select(x => x.ProjectFullPath)
					.Where(x => x.EndsWith(".UnitTests"));

				var changedProjectPaths = GitCompareReport.ChangedSolutions
					.SelectMany(x => x.ChangedProjects)
					.Select(x => x.ProjectFullPath)
					.Except(unitTestProjectsPaths);

				foreach (string? changedProject in changedProjectPaths)
				{
					string unitTestProjectName = Path.GetFileNameWithoutExtension(changedProject) + ".UnitTests";
					var unitTestProject = Solution!.GetProject(unitTestProjectName);
					if (unitTestProject is null)
					{
						Log.Warning($"Unit test project with name '{unitTestProjectName}' for '{changedProject}' not found.");
						continue;
					}

					unitTestProjectsPaths = unitTestProjectsPaths.Concat(new string[] { unitTestProject });
				}

				DotNetTest(_ => _
					.EnableNoRestore()
					.CombineWith(unitTestProjectsPaths,
						(settings, unitTestProject) => settings
							.SetProjectFile(unitTestProject)),
							degreeOfParallelism: 5);
			}
			else
			{
				Log.Error($"Git compare report unavailable. Running all unit tests.");
				var allUnitTestProjects = Solution!.GetProjects("*.UnitTests");
				DotNetTest(_ => _
					.EnableNoRestore()
					.CombineWith(allUnitTestProjects,
						(settings, unitTestProject) => settings
							.SetProjectFile(unitTestProject)),
							degreeOfParallelism: 5);
			}
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
