using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;
#pragma warning disable CA1002 // Do not expose generic lists

public class MessageRegistration
{
    public string? MessageDisplayName { get; set; }

    public string? GroupName { get; set; }

    public List<ParameterInfo> Parameters { get; } = new();

    public Type? ResponseRunTimeType { get; set; }

    public string? ResponseRunTimeTypeDisplayName { get; set; }

    public bool HasResponse => ResponseRunTimeType is not null;

    /// <summary>
    /// Optional.
    /// </summary>
    public string? HandlerUniqueName { get; set; }

    public RequestHandler? HandlerDelegate { get; set; }
}
