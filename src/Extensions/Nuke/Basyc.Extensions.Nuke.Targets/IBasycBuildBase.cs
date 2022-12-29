using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

namespace Basyc.Extensions.Nuke.Targets;
public interface IBasycBuildBase : INukeBuild
{
	public static string BuildProjectName { get; set; } = "_build";
	public static string UnitTestSuffix { get; set; } = ".UnitTests";

	[Solution] protected Solution Solution => TryGetValue(() => Solution);
	[GitVersion] protected GitVersion GitVersion => TryGetValue(() => GitVersion);

	protected AbsolutePath OutputDirectory => RootDirectory / "output";
	protected AbsolutePath OutputPackagesDirectory => OutputDirectory / "nugetPackages";
}
