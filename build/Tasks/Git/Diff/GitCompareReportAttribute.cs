using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.ValueInjection;
using System.Reflection;
using static _build.GitTasks;

namespace Tasks.Git.Diff;

[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class GitCompareReportAttribute : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		//Workaround();
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		var gitChanges = GitGetCompareReport(repository!.LocalDirectory);
		return gitChanges;
	}

	private static void Workaround()
	{
		var gitVersion = GitVersionTasks.GitVersion(s => s
				.SetFramework("net5.0")
				.SetNoFetch(false)
				.SetNoCache(false)
				.DisableProcessLogOutput()
				.SetUpdateAssemblyInfo(false))
			.Result;
	}
}
