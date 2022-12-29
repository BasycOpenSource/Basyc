using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildRelease : IBasycBuildBase
{
	protected string NugetSourceUrl { get; }
	protected string NuGetApiKey { get; }

	Target StaticCodeAnalysisAll => _ => _
	.Executes(() =>
	{
		Log.Information($"Running dotnet format for all files.");
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
		   Log.Information($"Running all unit tests.");
		   BasycUnitTestAndCoverageAll(Solution, UnitTestSuffix);
	   });

	Target NugetPushAll => _ => _
		   .Before(UnitTestAll)
		   .DependsOn(CompileAll)
		   .Executes(() =>
		   {
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
			   //var nugetPackages = packagesVersionedDirectory.GlobFiles("Basyc.Asp.*.nupkg");
			   DotNetNuGetPush(_ => _
				   .SetSource(NugetSourceUrl)
				   .SetApiKey(NuGetApiKey)
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));
		   });
}
