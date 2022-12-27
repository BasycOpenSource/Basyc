using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ValueInjection;
using System.Reflection;
using static Basyc.Extensions.Nuke.Tasks.GitTasks;

namespace Basyc.Extensions.Nuke.Tasks.Git.Diff;

[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class GitCompareReportAttribute : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		var gitChanges = GitGetCompareReport(repository!.LocalDirectory);
		return gitChanges;
	}
}
