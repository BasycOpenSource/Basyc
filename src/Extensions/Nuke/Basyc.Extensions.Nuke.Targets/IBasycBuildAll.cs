using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Tools.Git;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Basyc.Extensions.Nuke.Tasks.GitTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildAll : IBasycBuildBase
{
	[GitRepository] protected new GitRepository Repository => TryGetValue(() => Repository);

	Target PullRequestCheck => _ => _
	.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll)
	.OnlyWhenStatic(() => IsPullRequest)
	.Executes(() =>
	{
		if (GitFlowHelper.IsPullRequestAllowed(PullRequestSourceBranch, PullRequestTargetBranch) is false)
		{
			throw new InvalidOperationException($"Pull request between {PullRequestSourceBranch} and {PullRequestTargetBranch} is not allowed according git flow");
		}
	});

	Target ReleaseCheck => _ => _
	.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll)
	.OnlyWhenStatic(() => InvokedTargets.Any(x => x.Name == nameof(ReleaseAll)))
	.Executes(() =>
	{
		if (GitFlowHelper.IsReleaseAllowed(Repository.Branch) is false)
		{
			throw new InvalidOperationException($"Branch '{Repository.Branch}' is not allowed to be released according git flow. Only releases from main or develop are allowed");
		}
	});

	Target StaticCodeAnalysisAll => _ => _
	.Before(CompileAll)
	.Executes(() =>
	{
		BasycFormatVerifyNoChanges(Solution!.Path);
	});

	Target CleanAll => _ => _
	   .Before(RestoreAll)
	   .Executes(() =>
	   {
		   DotNetClean(_ => _
			   .SetProject(Solution));
	   });

	Target RestoreAll => _ => _
	   .Before(CompileAll)
	   .Executes(() =>
	   {
		   DotNetRestore(_ => _
			   .SetProjectFile(Solution));
	   });

	Target CompileAll => _ => _
	.DependsOn(RestoreAll)
	.After(StaticCodeAnalysisAll, RestoreAll)
	.Executes(() =>
	{
		using var tempSolution = SolutionHelper.NewTempSolution(Solution, BuildProjectName);
		DotNetBuild(_ => _
	   .EnableNoRestore()
	   .SetProjectFile(tempSolution.Solution));
	});

	Target UnitTestAll => _ => _
	   .DependsOn(CompileAll)
	   .Executes(() =>
	   {
		   BasycUnitTestAndCoverageAll(Solution, UnitTestSuffix);
	   });

	Target ReleaseAll => _ => _
		   .DependsOn(CompileAll)
		   .Before(UnitTestAll)
		   .Executes(() =>
		   {
			   GitCreateTag($"v{GitVersion!.NuGetVersionV2}", Repository);

			   using var solutionToUse = SolutionHelper.NewTempSolution(Solution, BuildProjectName);
			   var packagesVersionedDirectory = OutputPackagesDirectory / GitVersion!.NuGetVersionV2;

			   DotNetPack(_ => _
							.EnableNoRestore()
							.SetVersion(GitVersion!.NuGetVersionV2)
							.EnableNoBuild()
							.SetOutputDirectory(packagesVersionedDirectory)
								.SetProject(solutionToUse.Solution));

			   var nugetPackages = packagesVersionedDirectory.GlobFiles("*.nupkg");
			   DotNetNuGetPush(_ => _
				   .SetSource(NugetSourceUrl)
				   .SetApiKey(NuGetApiKey)
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));

		   });
}
