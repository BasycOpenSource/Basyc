using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.DotNetTasks;
using static Basyc.Extensions.Nuke.Tasks.Tools.Git.GitTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildCommonAll : IBasycBuildBase
{
	Target BranchCheckAll => _ => _
		.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll)
		.Executes(BranchCheck);

	Target PullRequestCheckAll => _ => _
		.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll)
		.OnlyWhenStatic(() => PullRequestSettings.IsPullRequest)
		.Executes(PullRequestCheck);

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
		.DependsOn(RestoreAll)
		.After(StaticCodeAnalysisAll, RestoreAll)
		.Executes(() =>
		{
			using var solutionToBuild = TemporarySolution.CreateNew(Solution, BuildProjectName);
			DotNetBuild(_ => _
				.EnableNoRestore()
				.SetProjectFile(solutionToBuild.Solution));
		});

	Target UnitTestAll => _ => _
		.DependsOn(CompileAll)
		.Executes(() =>
		{
			using var newCoverageReport = BasycUnitTestAll(Solution, UnitTestSettings.UnitTestSuffix, UnitTestSettings);
			if (PullRequestSettings.IsPullRequest)
			{
				Repository.TestsHistory.AddOrUpdateHistory(GitRepository.Branch!, newCoverageReport);
				Commit("[Automated] Adding test history ");
				Push();
			}


			Repository.TestsHistory.TryGetHistory(GitFlowHelper.GetSourceBranchType(GitRepository.Branch!).ToString(), out var oldCoverageReport);
			BasycTestCreateSummaryConsole(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum, oldCoverageReport);
			BasycTestAssertMinimum(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum);
		});
}
