using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentApiMessageRegistration
{
    public string? MessageDisplayName { get; set; }

    public MessageType MessageType { get; set; }

    public List<ParameterInfo> Parameters { get; } = new();

    public Type? ParametersFromType { get; set; }

    public bool ParametersAreFromType => ParametersFromType is not null;

    public RequestHandler? RequestHandler { get; set; }

    public Type? ResponseRunTimeType { get; set; }

    public string? ResponseRunTimeTypeDisplayName { get; set; }

    public bool HasResponse => ResponseRunTimeType is not null;
}
