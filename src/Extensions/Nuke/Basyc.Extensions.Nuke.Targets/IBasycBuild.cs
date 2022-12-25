using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.Dotnet.DotNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuild : INukeBuild
{
	[GitCompareReport] GitCompareReport GitCompareReport => TryGetValue(() => GitCompareReport);
	[Solution] Solution Solution => TryGetValue(() => Solution);
	[GitVersion] GitVersion GitVersion => TryGetValue(() => GitVersion);

	private GitHubActions GitHubActions => GitHubActions.Instance;
	private AbsolutePath OutputDirectory => RootDirectory / "output";
	private AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	Target StaticCodeAnalysis => _ => _
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

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			DotNetClean(_ => _
				.SetProject(Solution));
		});

	Target Restore => _ => _
		.Before(Compile)
		.Executes(() =>
		{
			DotNetRestore(_ => _
				.SetProjectFile(Solution));
		});

	Target Compile => _ => _
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

	Target UnitTest => _ => _
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

	Target NugetPush => _ => _
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
