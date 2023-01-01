using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Basyc.Extensions.Nuke.Tasks.Tools.Git;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Basyc.Extensions.Nuke.Tasks.GitTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets.Nuget;
public interface IBasycBuildNugetAll : IBasycBuildCommonAll
{
	protected string NugetSourceUrl { get; }
	protected string NuGetApiKey { get; }

	Target NugetReleaseAll => _ => _
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

	Target NugetReleaseCheck => _ => _
	.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll, NugetReleaseAll)
	.OnlyWhenStatic(() => InvokedTargets.Any(x => x.Name == nameof(NugetReleaseAll)))
	.Executes(() =>
	{
		if (GitFlowHelper.IsReleaseAllowed(Repository.Branch) is false)
		{
			throw new InvalidOperationException($"Branch '{Repository.Branch}' is not allowed to be released according git flow. Only releases from main or develop are allowed");
		}
	});
}
