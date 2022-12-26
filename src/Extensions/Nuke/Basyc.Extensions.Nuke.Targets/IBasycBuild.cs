using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
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
		//global::Nuke.Common.Tools.Coverlet.CoverletTasks.Coverlet()
		//Coverlet(_ => _
		//.SetTarget("dotnet")
		//.SetTargetArgs(@"test C:\Users\Honza\source\repos\BasycOpenSource\Basyc\src\Serialization\Basyc.Serialization.ProtobufNet\Basyc.Serialization.ProtobufNet.csproj")
		//.SetOutput(@"C:\Users\Honza\AppData\Local\Temp\CoverletOutput.json")
		//.SetAssembly(@"C:\Users\Honza\source\repos\BasycOpenSource\Basyc\tests\Serialization\Basyc.Serialization.ProtobufNet.UnitTests\bin\Debug\net7.0\Basyc.Serialization.ProtobufNet.UnitTests.dll"));

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
			   //Solution.prop
			   if (GitCompareReport!.CouldCompare)
			   {
				   var changedProjects = GitCompareReport.ChangedSolutions
				   .SelectMany(x => x.ChangedProjects)
				   .Select(x => x.ProjectFullPath)
				   .Where(x => x.EndsWith("_build.csproj") is false);

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

	//https://github.com/danielpalme/ReportGenerator
	Target UnitTest => _ => _
		   .DependsOn(Compile)
		   .Executes(() =>
		   {
			   string unitTestSuffix = ".UnitTests";
			   if (GitCompareReport!.CouldCompare)
			   {
				   var testProjectsToRun = GitCompareReport.GetTestProjectsToRun(Solution, unitTestSuffix);

				   Log.Information($"Starting unit tests: '{string.Join("\n", testProjectsToRun)}'");
				   UnitTestAndCoverage(testProjectsToRun, unitTestSuffix);
			   }
			   else
			   {
				   Log.Error($"Git compare report unavailable. Running all unit tests.");
				   var allUnitTestProjects = Solution!.GetProjects("*.UnitTests");
				   UnitTestAndCoverage(allUnitTestProjects.Select(x => x.Path.ToString()), unitTestSuffix);

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
