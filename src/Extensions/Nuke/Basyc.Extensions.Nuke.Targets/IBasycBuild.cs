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
			StaticCodeAnalysisAll.Invoke(_);
		}
	});

	Target StaticCodeAnalysisAll => _ => _
		.Before(targets: Compile)
		.Executes(() =>
		{

			Log.Error($"Running dotnet format for all files.");
			DotnetFormatVerifyNoChanges(Solution!.Path);
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
				   .Select(x => x.ProjectFullPath)
				   .Where(x => x.EndsWith($"{BuildProjectName}.csproj") is false);

				   DotNetBuild(_ => _
					   .EnableNoRestore()
					   .CombineWith(changedProjects, (_, changedProject) => _
					   .SetProjectFile(changedProject)));
			   }
			   else
			   {
				   var changedProjects = Solution.Projects.Where(x => x.Name.EndsWith($"{BuildProjectName}.csproj") is false);

				   Log.Error($"Git compare report unavailable. Building whole solution.");
				   DotNetBuild(_ => _
					   .EnableNoRestore()
					   .CombineWith(changedProjects, (_, changedProject) => _
						.SetProjectFile(changedProject)));
			   }
		   });

	//https://github.com/danielpalme/ReportGenerator
	Target UnitTestAffected => _ => _
		   .DependsOn(Compile)
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
	   .DependsOn(Compile)
	   .Executes(() =>
	   {
		   Log.Error($"Running all unit tests.");
		   UnitTestAndCoverageAll(Solution, UnitTestSuffix);
	   });

	Target NugetPush => _ => _
		   .Before(UnitTestAffected)
		   .Executes(() =>
		   {
			   var projectsToPublish = Solution.Projects.Where(x => x.Name.EndsWith($"{BuildProjectName}.csproj") is false);

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
				   .SetSource(NuGetSource)
				   .SetApiKey(NuGetApiKey)
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));
		   });
}
