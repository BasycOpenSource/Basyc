using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Serilog;
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
			BasycFormatVerifyNoChanges(Solution.Path);
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
		.Executes(async () =>
		{
			using var newCoverageReport = BasycUnitTestAll(Solution, UnitTestSettings.UnitTestSuffix, UnitTestSettings);
			// using var newCoverageReport = new CoverageReport(IO.TemporaryDirectory.CreateNew(), Array.Empty<ProjectCoverageReport>());
			if (UnitTestSettings.PublishResults)
			{
				Log.Information("Publishing test results");
				var newHistoryFilePath = Repository.TestsHistory.AddOrUpdateHistory(GitRepository.Branch.Value(), newCoverageReport);
				var credentials = await GetCredentialsWindows();
				Commit("[Automated] Adding test history", newHistoryFilePath);
				Push(credentials);
				Log.Information("Test results published");
			}

			var sourceBranchToCompare = PullRequestSettings.IsPullRequest
				? PullRequestSettings.SourceBranch.Value()
				: GitFlowHelper.GetSourceBranch(GitRepository.Branch.Value()).Name;

			Repository.TestsHistory.TryGetHistory(sourceBranchToCompare, out var oldCoverageReport);
			BasycTestCreateSummaryConsole(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum, oldCoverageReport);
			BasycTestAssertMinimum(newCoverageReport, UnitTestSettings.SequenceMinimum, UnitTestSettings.BranchMinimum);
		});
}
