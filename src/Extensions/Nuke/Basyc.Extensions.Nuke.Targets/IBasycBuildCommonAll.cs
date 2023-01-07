using Basyc.Extensions.Nuke.Tasks.Helpers;
using Basyc.Extensions.Nuke.Tasks.Helpers.Solutions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildCommonAll : IBasycBuildBase
{
	[GitRepository] protected new GitRepository Repository => TryGetValue(() => Repository);

	Target PullRequestCheck => _ => _
	.DependentFor(StaticCodeAnalysisAll, CleanAll, RestoreAll, CompileAll, UnitTestAll, RestoreAll)
	.OnlyWhenStatic(() => IsPullRequest)
	.Executes(() =>
	{
		if (GitFlowHelper.IsPullRequestAllowed(PullRequestSourceBranch, PullRequestTargetBranch, false) is false)
		{
			throw new InvalidOperationException($"Pull request between {PullRequestSourceBranch} and {PullRequestTargetBranch} is not allowed according git flow");
		}
	});

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
		using var tempSolution = SolutionHelper.NewTempSolution(Solution, BuildProjectName);
		DotNetBuild(_ => _
	   .EnableNoRestore()
	   .SetProjectFile(tempSolution.Solution));
	});

	Target UnitTestAll => _ => _
	   .DependsOn(CompileAll)
	   .Executes(() =>
	   {
		   BasycUnitTestAll(Solution, UnitTestSuffix);
	   });

}
