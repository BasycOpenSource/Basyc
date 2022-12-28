using Basyc.Extensions.Nuke.Tasks.Git.Diff;
using Nuke.Common;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildAffected : IBasycBuildAll
{
	[GitCompareReport] GitCompareReport GitCompareReport => TryGetValue(() => GitCompareReport);

	Target StaticCodeAnalysisAffected => _ => _
	.Before(CompileAffected)
	.Executes(() =>
	{
		if (GitCompareReport!.CouldCompare)
		{
			BasycFormatVerifyNoChangesAffected(GitCompareReport!);
		}
		else
		{
			Log.Error($"Git compare report unavailable. Running dotnet format for all files.");
			StaticCodeAnalysisAll.Invoke(_);
		}
	});
	Target RestoreAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{
			if (GitCompareReport!.CouldCompare)
			{
				BasycRestoreAffected(GitCompareReport, UnitTestSuffix, BuildProjectName, Solution);
			}
			else
			{
				Log.Error($"Git compare report unavailable. Restoring all.");
				RestoreAll.Invoke(_);
			}
		});

	Target CompileAffected => _ => _
		   .DependsOn(RestoreAffected)
		   .Executes(() =>
		   {
			   if (GitCompareReport!.CouldCompare)
			   {
				   BasycBuildAffected(GitCompareReport, UnitTestSuffix, BuildProjectName, Solution);
			   }
			   else
			   {
				   Log.Error($"Git compare report unavailable. Building whole solution.");
				   CompileAll.Invoke(_);
			   }
		   });

	//https://github.com/danielpalme/ReportGenerator
	Target UnitTestAffected => _ => _
		   .DependsOn(CompileAffected)
		   .Executes(() =>
		   {
			   if (GitCompareReport!.CouldCompare)
			   {
				   Log.Information($"Starting only affected unit tests");
				   BasycUnitTestAndCoverageAffected(Solution, GitCompareReport, UnitTestSuffix);
			   }
			   else
			   {
				   Log.Error($"Git compare report unavailable. Running all unit tests.");
				   UnitTestAll.Invoke(_);
			   }
		   });

}
