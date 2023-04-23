using System.Reflection;

namespace Basyc.MessageBus.Manager.Application;

public interface ITypedDomainNameFormatter
{
    string GetFormattedName(Assembly assembly);
}
