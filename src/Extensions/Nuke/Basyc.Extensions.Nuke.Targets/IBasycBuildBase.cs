using Basyc.Extensions.Nuke.Tasks.Tools.Dotnet.Test;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildBase : INukeBuild
{
	string BuildProjectName { get; }
	UnitTestSettings UnitTestSettings { get; }

	//[Solution(SuppressBuildProjectCheck = true)] public Solution Solution => TryGetValue(() => Solution);
	public Solution Solution { get; }
	[GitVersion] public GitVersion GitVersion => TryGetValue(() => GitVersion);
	[GitRepository] public GitRepository Repository => TryGetValue(() => Repository);

	public AbsolutePath OutputDirectory => RootDirectory / "output";
	public AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";
	public AbsolutePath TestHistoryDirectory => RootDirectory / "tests" / "history";

	public bool IsPullRequest { get; }
	public string PullRequestTargetBranch { get; }
	public string PullRequestSourceBranch { get; }
}
