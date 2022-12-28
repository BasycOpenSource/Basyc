using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Targets.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuild : INukeBuild
{
	protected static string BuildProjectName { get; set; } = "_build";
	protected static string UnitTestSuffix { get; set; } = ".UnitTests";

	[GitCompareReport] GitCompareReport GitCompareReport => TryGetValue(() => GitCompareReport);
	[Solution] Solution Solution => TryGetValue(() => Solution);
	[GitVersion] GitVersion GitVersion => TryGetValue(() => GitVersion);
	[Parameter][Secret] string NuGetApiKey => TryGetValue(() => NuGetApiKey);
	[Parameter] string NuGetSource => TryGetValue(() => NuGetSource);

	private AbsolutePath OutputDirectory => RootDirectory / "output";
	private AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	Target StaticCodeAnalysisAffected => _ => _
	.Before(CompileAffected)
	.Executes(() =>
	{
		if (GitCompareReport!.CouldCompare)
		{
			DotnetFormatVerifyNoChanges(GitCompareReport!);
		}
		else
		{
			Log.Error($"Git compare report unavailable. Running dotnet format for all files.");
			StaticCodeAnalysisAll.Invoke(_);
		}
	});

	Target StaticCodeAnalysisAll => _ => _
		.Before(targets: CompileAffected)
		.Executes(() =>
		{
			Log.Information($"Running dotnet format for all files.");
			DotnetFormatVerifyNoChanges(Solution!.Path);
		});

	Target CleanAll => _ => _
		   .Before(RestoreAll)
		   .Executes(() =>
		   {
			   DotNetClean(_ => _
				   .SetProject(Solution));
		   });

	Target RestoreAll => _ => _
		   .Before(CompileAffected)
		   .Executes(() =>
		   {
			   DotNetRestore(_ => _
				   .SetProjectFile(Solution));
		   });

	Target CompileAffected => _ => _
		   .After(RestoreAll)
		   .DependsOn(RestoreAll)
		   .Executes(() =>
		   {
			   if (GitCompareReport!.CouldCompare)
			   {
				   var changedProjectsPaths = GitCompareReport.ChangedSolutions
				   .SelectMany(x => x.ChangedProjects)
				   .Select(x => x.ProjectFullPath);

				   changedProjectsPaths = changedProjectsPaths.Concat(changedProjectsPaths.Select(x =>
				   {
					   var testProject = Solution.GetProject(Path.GetFileNameWithoutExtension(x) + UnitTestSuffix);
					   if (testProject is null)
					   {
						   return null!;
					   }

					   return testProject.Path.ToString().NormalizePath();
				   }).Where(x => x is not null));
				   using var solutionToUse = SolutionHelper.NewTempSolution(Solution, BuildProjectName, changedProjectsPaths);

				   DotNetBuild(_ => _
					.EnableNoRestore()
					.SetProjectFile(solutionToUse.Solution));
			   }
			   else
			   {
				   Log.Error($"Git compare report unavailable. Building whole solution.");
				   CompileAll.Invoke(_);
			   }
		   });

	Target CompileAll => _ => _
		   .After(RestoreAll)
		   .DependsOn(RestoreAll)
		   .Executes(() =>
		   {
			   using var tempSolution = SolutionHelper.NewTempSolution(Solution, BuildProjectName);

			   DotNetBuild(_ => _
				.EnableNoRestore()
				.SetProjectFile(tempSolution.Solution));

		   });

	//https://github.com/danielpalme/ReportGenerator
	Target UnitTestAffected => _ => _
		   .DependsOn(CompileAffected)
		   .Executes(() =>
		   {
			   if (GitCompareReport!.CouldCompare)
			   {
				   Log.Information($"Starting only affected unit tests");
				   UnitTestAndCoverageAffected(Solution, GitCompareReport, UnitTestSuffix);
			   }
			   else
			   {
				   Log.Error($"Git compare report unavailable. Running all unit tests.");
				   UnitTestAll.Invoke(_);
			   }
		   });

	Target UnitTestAll => _ => _
	   .DependsOn(CompileAll)
	   .Executes(() =>
	   {
		   Log.Information($"Running all unit tests.");
		   UnitTestAndCoverageAll(Solution, UnitTestSuffix);
	   });

	Target NugetPushAll => _ => _
		   .Before(UnitTestAll)
		   .DependsOn(CompileAll)
		   .Executes(() =>
		   {
			   var projectsToPublish = Solution.AllProjects.Where(x => x.Path.ToString().EndsWith($"{BuildProjectName}.csproj") is false);

			   //DotNetPack(_ => _
			   // .EnableNoRestore()
			   // .SetVersion(GitVersion!.NuGetVersionV2)
			   // .EnableNoBuild()
			   // .SetProject(Solution)
			   // .SetOutputDirectory(OutputPackagesDirectory));

			   DotNetPack(_ => _
					.EnableNoRestore()
					.SetVersion(GitVersion!.NuGetVersionV2)
					.EnableNoBuild()
					.SetOutputDirectory(OutputPackagesDirectory)
					.CombineWith(projectsToPublish, (_, project) => _
						.SetProject(project)));

			   var nugetPackages = OutputPackagesDirectory.GlobFiles("*.nupkg");

			   DotNetNuGetPush(_ => _
				   .SetSource("https://api.nuget.org/v3/index.json")
				   .SetApiKey("oy2lz2o2kfxbcgrktvjaq3vdnn4fptvuhmvey6x2enz6wi")
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));
		   });
}
