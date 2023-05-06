using Basyc.MessageBus.Manager.Application;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedRequestNameFormatter : ITypedRequestNameFormatter
{
    public string GetFormattedName(Type requestType)
    {
        string requestName = requestType.Name
            .Replace("Command", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Request", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Message", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Query", string.Empty, StringComparison.OrdinalIgnoreCase);

        return requestName;
    }
}
