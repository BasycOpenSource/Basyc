using System.Reflection;
using Basyc.MessageBus.Manager.Application;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class InterfaceRegistration
{
    public const string DefaultRequestHandlerUniqueName = "DefaultRequester";

    public List<Assembly> AssembliesToScan { get; init; } = new();

    public string? GroupName { get; set; }

    public Type? MessageInterfaceType { get; set; }

    public bool HasResponse { get; set; }

    public Type? ResponseType { get; set; }

    public Func<Type, string>? DisplayNameFormatter { get; set; }

    public string? ResponseDisplayName { get; set; }

    public MessageType RequestType { get; set; }

    public string? RequestHandlerUniqueName { get; set; } = DefaultRequestHandlerUniqueName;
}
