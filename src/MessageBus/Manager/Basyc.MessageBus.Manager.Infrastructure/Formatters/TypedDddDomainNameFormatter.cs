using Basyc.MessageBus.Manager.Application;
using System.Reflection;
using Throw;

namespace Basyc.MessageBus.Manager;

public class TypedDddDomainNameFormatter : ITypedDomainNameFormatter
{
    public string GetFormattedName(Assembly assembly)
    {
        var assemblyName = assembly.GetName();
        assemblyName.ThrowIfNull();
        assemblyName.Name.ThrowIfNull();

        var customName = assemblyName.Name.Split('.')[^2];
        return customName;
    }
}
