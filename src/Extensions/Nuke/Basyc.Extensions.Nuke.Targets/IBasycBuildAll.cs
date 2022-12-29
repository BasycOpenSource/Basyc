using Basyc.Extensions.IO;
using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildAll : INukeBuild
{
	public static string BuildProjectName { get; set; } = "_build";
	public static string UnitTestSuffix { get; set; } = ".UnitTests";

	[Solution] Solution Solution => TryGetValue(() => Solution);
	[GitVersion] GitVersion GitVersion => TryGetValue(() => GitVersion);
	[Parameter] string NuGetSource => TryGetValue(() => NuGetSource);
	[Parameter][Secret] string NuGetApiKey => TryGetValue(() => NuGetApiKey);
	[Parameter][Secret] string NuGetApiPrivateKeyPfxBase64 => TryGetValue(() => NuGetApiPrivateKeyPfxBase64);
	[Parameter][Secret] string NuGetApiCertPassword => TryGetValue(() => NuGetApiCertPassword);

	protected AbsolutePath OutputDirectory => RootDirectory / "output";
	protected AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

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

			   byte[] certContent = Convert.FromBase64String(NuGetApiPrivateKeyPfxBase64);
			   using var cert = TemporaryFile.CreateNewWith(fileExtension: "pfx", content: certContent);
			   string pathToSign = (packagesVersionedDirectory.ToString() + "/*.nupkg").NormalizeForCurrentOs();
			   BasycNugetSign(pathToSign, cert.FullPath, NuGetApiCertPassword);

			   var nugetPackages = packagesVersionedDirectory.GlobFiles("*.nupkg");
			   DotNetNuGetPush(_ => _
				   .SetSource(NuGetSource)
				   .SetApiKey(NuGetApiKey)
				   .CombineWith(nugetPackages, (_, nugetPackage) => _
					   .SetTargetPath(nugetPackage)));
		   });
}
