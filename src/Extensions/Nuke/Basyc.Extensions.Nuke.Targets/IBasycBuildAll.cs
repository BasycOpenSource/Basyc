using Basyc.Extensions.Nuke.Tasks.Git;
using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Basyc.Extensions.Nuke.Tasks.GitTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildAll : IBasycBuildBase
{
	[GitRepository] protected new GitRepository Repository => TryGetValue(() => Repository);

	Target PullRequestCheck => _ => _
		.Executes(() =>
		{
			if (IsPullRequest is false)
			{
				throw new InvalidOperationException($"Can't validate pull request if {IsPullRequest} is false");
			}

			if (BranchHelper.IsPullRequestAllowed(Repository.Branch, PullRequestTargetBranch) is false)
			{
				throw new InvalidOperationException($"Pull request between {Repository.Branch} and {PullRequestTargetBranch} is not allowed");
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
	   .After(StaticCodeAnalysisAll)
	   .After(RestoreAll)
	   .DependsOn(RestoreAll)
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
		   .Before(UnitTestAll)
		   .DependsOn(CompileAll)
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

			   //string pathToSign = (packagesVersionedDirectory.ToString() + "/*.nupkg").NormalizeForCurrentOs();
			   //BasycNugetSignWithBase64(pathToSign, NuGetApiPrivateKeyPfxBase64, NuGetApiCertPassword);

			   var nugetPackages = packagesVersionedDirectory.GlobFiles("*.nupkg");
			   DotNetNuGetPush(_ => _
				   .SetSource(NugetSourceUrl)
				   .SetApiKey(NuGetApiKey)
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));

		   });
}
