using Nuke.Common;
using Nuke.Common.ValueInjection;
using System.Reflection;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Structure;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class RepositoryAttribute : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		return new Repository(NukeBuild.RootDirectory);
	}
}
