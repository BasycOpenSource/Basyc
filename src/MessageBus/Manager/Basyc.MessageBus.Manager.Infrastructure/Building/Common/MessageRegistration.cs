using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public class MessageRegistration
{
    public string? MessageDisplayName { get; set; }
    public string? GroupName { get; set; }
    public List<ParameterInfo> Parameters { get; } = new();
    public Type? ResponseRunTimeType { get; set; }
    public string? ResponseRunTimeTypeDisplayName { get; set; }
    public bool HasResponse => ResponseRunTimeType is not null;
    /// <summary>
    /// Optional
    /// </summary>
    public string? HandlerUniqueName { get; set; }
    public RequestHandlerDelegate? HandlerDelegate { get; set; }
}
