using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common;
using Serilog;
using System.Collections;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildCommonAffected : IBasycBuildBase
{
	[AffectedReport] RepositoryChangeReport RepositoryChangeReport => TryGetValue(() => RepositoryChangeReport)!;

	[Parameter]
	string nugetSource => TryGetValue(() => nugetSource);

	Target BranchCheckAffected => _ => _
		.DependentFor(StaticCodeAnalysisAffected, RestoreAffected, CompileAffected, UnitTestAffected, RestoreAffected)
		.Executes(BranchCheck);

	Target ChangeReportCheck => _ => _
		.DependentFor(StaticCodeAnalysisAffected, RestoreAffected, CompileAffected, UnitTestAffected, RestoreAffected)
		.Executes(RepositoryChangeReport.ThrowIfNotValid);

	Target StaticCodeAnalysisAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{


			//TODO remove logging env vars
			foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
				Log.Information($"{environmentVariable.Key}-{environmentVariable.Value}");


			Log.Information($"nugetSource is : {nugetSource}");
			Log.Information($"nugetSource contains dummy : {nugetSource.Contains("DUMMY VALUE")}");
			Log.Information($"nugetSource contains _ : {nugetSource.Contains("_")}");
			BasycDotNetFormatVerifyNoChangesAffected(RepositoryChangeReport!);
		});

	Target RestoreAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{
			BasycDotNetRestoreAffected(RepositoryChangeReport, UnitTestSettings.UnitTestSuffix, BuildProjectName, Solution);
		});

	Target CompileAffected => _ => _
		.DependsOn(RestoreAffected)
		.Executes(() =>
		{
			BasycDotNetBuildAffected(RepositoryChangeReport, UnitTestSettings.UnitTestSuffix, BuildProjectName, Solution);
		});

	Target UnitTestAffected => _ => _
		.DependsOn(CompileAffected)
		.Executes(() =>
		{
			using var newCoverageReport = BasycUnitTestAffected(Solution, RepositoryChangeReport, UnitTestSettings.UnitTestSuffix, UnitTestSettings);
			Repository.TestsHistory.TryGetHistory(GitFlowHelper.GetSourceBranch(GitRepository.Branch.Value()).Name, out var oldCoverageReport);
			BasycTestCreateSummaryConsole(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum, oldCoverageReport);
			BasycTestAssertMinimum(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum);
		});
}
