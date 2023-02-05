using Basyc.MessageBus.Manager.Application;
using System.Reflection;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainNameFormatter : ITypedDomainNameFormatter
{
	public string GetFormattedName(Assembly assembly)
	{
		var assemblyName = assembly.GetName();
		assemblyName.ThrowIfNull();
		assemblyName.Name.ThrowIfNull();
		return assemblyName.Name;
	}
}
