using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildBase : INukeBuild
{
	string BuildProjectName { get; }
	string UnitTestSuffix { get; }

	[Solution(SuppressBuildProjectCheck = true)] protected Solution Solution => TryGetValue(() => Solution);
	[GitVersion] protected GitVersion GitVersion => TryGetValue(() => GitVersion);
	[GitRepository] protected GitRepository Repository => TryGetValue(() => Repository);

	protected AbsolutePath OutputDirectory => RootDirectory / "output";
	protected AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";
	protected AbsolutePath TestHistoryDirectory => RootDirectory / "tests" / "history";

	protected bool IsPullRequest { get; }
	protected string PullRequestTargetBranch { get; }
	protected string PullRequestSourceBranch { get; }

	double MinSequenceCoverage { get; }
	double MinBranchCoverage { get; }
}
