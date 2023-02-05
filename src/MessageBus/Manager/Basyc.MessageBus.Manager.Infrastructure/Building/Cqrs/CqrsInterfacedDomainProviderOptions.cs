using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class CqrsInterfacedDomainProviderOptions
{
	private readonly List<CqrsRegistration> cqrsRegistrations = new();

	public void AddcqrsRegistration(CqrsRegistration cqrsRegistration)
	{
		cqrsRegistrations.Add(cqrsRegistration);
	}

	public List<CqrsRegistration> GetcqrsRegistrations()
	{
		return cqrsRegistrations;
	}

	public class CqrsRegistration
	{
		public Type? QueryInterfaceType { get; set; }
		public Type? CommandInterfaceType { get; set; }
		public Type? CommandWithResponseInterfaceType { get; set; }
		public Type? MessageInterfaceType { get; set; }
		public Type? MessageWithResponseInterfaceType { get; set; }
		public string? DomainName { get; set; }

		public IEnumerable<Assembly> AssembliesToScan { get; set; } = new List<Assembly>();
	}
}
