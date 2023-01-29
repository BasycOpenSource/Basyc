using Basyc.Extensions.Nuke.Tasks.Helpers.GitFlow;
using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Serilog;

namespace Basyc.Extensions.Nuke.Targets;

public interface IBasycBuildBase : INukeBuild
{
	string BuildProjectName { get; }
	UnitTestSettings UnitTestSettings { get; }
	public Solution Solution { get; }
	[GitVersion] public GitVersion GitVersion => TryGetValue(() => GitVersion)!;
	[GitRepository] public GitRepository Repository => TryGetValue(() => Repository)!;

	public AbsolutePath OutputDirectory => RootDirectory / "output";
	public AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";
	public AbsolutePath TestHistoryDirectory => RootDirectory / "tests" / "history";

	public bool IsPullRequest { get; }
	public string PullRequestTargetBranch { get; }
	public string PullRequestSourceBranch { get; }

	protected void BranchCheck()
	{
		if (GitFlowHelper.IsGitFlowBranch(Repository.Branch!) is false)
		{
			throw new InvalidOperationException(
				$"Branch '{Repository.Branch!}' is not allowed branch name according git flow");
		}

		Log.Information($"Branch name '{Repository.Branch!}' is valid git flow branch");
	}

	protected void PullRequestCheck()
	{
		if (GitFlowHelper.IsPullRequestAllowed(PullRequestSourceBranch, PullRequestTargetBranch) is false)
		{
			throw new InvalidOperationException(
				$"Pull request between {PullRequestSourceBranch} and {PullRequestTargetBranch} is not allowed according git flow");
		}

		Log.Information($"Pull request between '{PullRequestSourceBranch}' nad '{PullRequestTargetBranch}' and is valid according git flow");
	}
}
