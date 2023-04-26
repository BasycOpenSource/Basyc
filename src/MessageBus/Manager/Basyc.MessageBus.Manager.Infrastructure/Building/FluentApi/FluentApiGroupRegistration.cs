namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentApiGroupRegistration
{
    public string? Name { get; set; }

    public ICollection<FluentApiMessageRegistration> InProgressMessages { get; init; } = new List<FluentApiMessageRegistration>();
}
