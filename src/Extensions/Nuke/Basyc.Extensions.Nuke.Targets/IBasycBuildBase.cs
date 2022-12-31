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

	[Solution] protected Solution Solution => TryGetValue(() => Solution);
	[GitVersion] protected GitVersion GitVersion => TryGetValue(() => GitVersion);
	[GitRepository] protected GitRepository Repository => TryGetValue(() => Repository);

	protected AbsolutePath OutputDirectory => RootDirectory / "output";
	protected AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";

	protected bool IsPullRequest { get; }
	protected string PullRequestTargetBranch { get; }

	protected string NugetSourceUrl { get; }
	protected string NuGetApiKey { get; }
}
