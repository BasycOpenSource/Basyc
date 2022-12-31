using Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Serilog;
using static Basyc.Extensions.Nuke.Tasks.DotNetTasks;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildAffected : IBasycBuildBase
{
	[GitCompareReport] GitCompareReport GitCompareReport => TryGetValue(() => GitCompareReport);

	Target StaticCodeAnalysisAffected => _ => _
	.Before(CompileAffected)
	.Executes(() =>
	{
		Log.Information(GitHubActions.Instance.ServerUrl);
		Log.Information(GitHubActions.Instance.Repository);
		GitCompareReport.ThrowIfNotValid();
		BasycFormatVerifyNoChangesAffected(GitCompareReport!);
	});
	Target RestoreAffected => _ => _
		.Before(CompileAffected)
		.Executes(() =>
		{
			GitCompareReport.ThrowIfNotValid();
			BasycRestoreAffected(GitCompareReport, UnitTestSuffix, BuildProjectName, Solution);
		});

	Target CompileAffected => _ => _
		   .DependsOn(RestoreAffected)
		   .Executes(() =>
		   {
			   GitCompareReport.ThrowIfNotValid();
			   BasycBuildAffected(GitCompareReport, UnitTestSuffix, BuildProjectName, Solution);
		   });

	//https://github.com/danielpalme/ReportGenerator
	Target UnitTestAffected => _ => _
		   .DependsOn(CompileAffected)
		   .Executes(() =>
		   {
			   GitCompareReport.ThrowIfNotValid();
			   BasycUnitTestAndCoverageAffected(Solution, GitCompareReport, UnitTestSuffix);
		   });

}
