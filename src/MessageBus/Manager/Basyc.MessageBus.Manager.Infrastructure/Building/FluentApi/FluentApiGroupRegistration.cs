namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentApiGroupRegistration
{
    public string? Name { get; set; }
    public List<FluentApiMessageRegistration> InProgressMessages { get; } = new();
}
