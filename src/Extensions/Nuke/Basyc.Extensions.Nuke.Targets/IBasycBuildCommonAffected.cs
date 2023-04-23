using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildCommonAffected : IBasycBuildBase
{
    [AffectedReport]
    RepositoryChangeReport RepositoryChangeReport => TryGetValue(() => RepositoryChangeReport)!;

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
