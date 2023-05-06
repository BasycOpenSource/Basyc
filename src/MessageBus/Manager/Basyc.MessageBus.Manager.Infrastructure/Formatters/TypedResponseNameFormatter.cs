using Basyc.MessageBus.Manager.Application;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedResponseNameFormatter : ITypedResponseNameFormatter
{
    public string GetFormattedName(Type responseType) => responseType.Name
            .Replace("Command", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Request", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Message", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Query", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Result", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Response", string.Empty, StringComparison.OrdinalIgnoreCase);
}
