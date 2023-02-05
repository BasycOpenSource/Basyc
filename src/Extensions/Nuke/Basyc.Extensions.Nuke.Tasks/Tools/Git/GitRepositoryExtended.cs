using Nuke.Common.ValueInjection;
using System.Reflection;

namespace Basyc.Extensions.Nuke.Tasks.Tools.Git;

public class GitRepositoryExtended : ValueInjectionAttributeBase
{
	public override object GetValue(MemberInfo member, object instance)
	{
		throw new NotImplementedException();
	}
}
