using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildCommonAffected : IBasycBuildBase
{
	[AffectedReport] AffectedReport AffectedReport => TryGetValue(() => AffectedReport);

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
			BasycDotNetRestoreAffected(AffectedReport, UnitTestSuffix, BuildProjectName, Solution);
		});

	Target CompileAffected => _ => _
		   .DependsOn(RestoreAffected)
		   .Executes(() =>
		   {
			   AffectedReport.ThrowIfNotValid();
			   BasycDotNetBuildAffected(AffectedReport, UnitTestSuffix, BuildProjectName, Solution);
		   });

	Target UnitTestAffected => _ => _
		   .DependsOn(CompileAffected)
		   .Executes(() =>
		   {
			   AffectedReport.ThrowIfNotValid();
			   using var coverageReport = BasycUnitTestAffected(Solution, AffectedReport, UnitTestSuffix);
			   string oldCoverageFile = (TestHistoryDirectory / "develop") + ".json";
			   if (File.Exists(oldCoverageFile))
			   {
				   using var oldCoverage = BasycTestLoadFromFile(oldCoverageFile);
				   BasycTestCreateSummaryConsole(coverageReport, MinSequenceCoverage, MinBranchCoverage, oldCoverage);
			   }
			   else
			   {
				   BasycTestCreateSummaryConsole(coverageReport, MinSequenceCoverage, MinBranchCoverage);
			   }

			   BasycTestAssertMinimum(coverageReport, MinSequenceCoverage, MinBranchCoverage);
		   });
}

