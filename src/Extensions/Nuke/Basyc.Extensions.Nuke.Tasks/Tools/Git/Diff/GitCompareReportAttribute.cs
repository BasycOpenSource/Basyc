using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ValueInjection;
using System.Reflection;
using static Basyc.Extensions.Nuke.Tasks.Tools.Git.GitTasks;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git.Diff;

[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class AffectedReportAttribute : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		var repository = GitRepository.FromLocalDirectory(NukeBuild.RootDirectory);
		var gitChanges = GitGetAffectedReport(repository!.LocalDirectory);
		return gitChanges;
	}
}
