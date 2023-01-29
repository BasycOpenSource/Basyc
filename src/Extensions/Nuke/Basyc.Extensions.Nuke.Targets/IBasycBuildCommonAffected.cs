using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildCommonAffected : IBasycBuildBase
{
	[AffectedReport] AffectedReport AffectedReport => TryGetValue(() => AffectedReport)!;

	Target BranchCheckAffected => _ => _
		.DependentFor(StaticCodeAnalysisAffected, RestoreAffected, CompileAffected, UnitTestAffected, RestoreAffected)
		.Executes(BranchCheck);

	Target PullRequestCheckAffected => _ => _
		.DependentFor(StaticCodeAnalysisAffected, RestoreAffected, CompileAffected, UnitTestAffected, RestoreAffected)
		.OnlyWhenStatic(() => IsPullRequest)
		.Executes(PullRequestCheck);

	Target StaticCodeAnalysisAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{
			AffectedReport.ThrowIfNotValid();
			BasycDotNetFormatVerifyNoChangesAffected(AffectedReport!);
		});

	Target RestoreAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{
			AffectedReport.ThrowIfNotValid();
			BasycDotNetRestoreAffected(AffectedReport, UnitTestSettings.UnitTestSuffix, BuildProjectName, Solution);
		});

	Target CompileAffected => _ => _
		.DependsOn(RestoreAffected)
		.Executes(() =>
		{
			AffectedReport.ThrowIfNotValid();
			BasycDotNetBuildAffected(AffectedReport, UnitTestSettings.UnitTestSuffix, BuildProjectName, Solution);
		});

	Target UnitTestAffected => _ => _
		.DependsOn(CompileAffected)
		.Executes(() =>
		{
			AffectedReport.ThrowIfNotValid();
			var oldCoverageFile =
				$"{TestHistoryDirectory / GitFlowHelper.GetGitFlowSourceBranch(Repository.Branch!).ToString()}.json";
			using var coverageReport = BasycUnitTestAffected(Solution, AffectedReport, UnitTestSettings.UnitTestSuffix,
				UnitTestSettings);
			if (File.Exists(oldCoverageFile))
			{
				var newCoverageFile = $"{TestHistoryDirectory / Repository.Branch!.Replace('/', '-')}.json";
				using var oldCoverage = BasycCoverageLoadFromFile(oldCoverageFile);
				BasycCoverageSaveToFile(oldCoverage, newCoverageFile);
				BasycTestCreateSummaryConsole(coverageReport, UnitTestSettings.SequenceMinimum,
					UnitTestSettings.BranchMinimum, oldCoverage);
			}
			else
			{
				BasycTestCreateSummaryConsole(coverageReport, UnitTestSettings.SequenceMinimum,
					UnitTestSettings.BranchMinimum);
			}

			BasycTestAssertMinimum(coverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum);
		});
}
