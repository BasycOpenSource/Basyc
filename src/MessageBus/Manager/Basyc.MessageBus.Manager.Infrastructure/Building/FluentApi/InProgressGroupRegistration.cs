namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class InProgressGroupRegistration
{
	public string? DomainName { get; set; }
	public List<InProgressMessageRegistration> InProgressMessages { get; } = new();
}
