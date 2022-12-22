using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application;

public class FluentApiDomainInfoProviderOptions
{
	public List<InProgressDomainRegistration> DomainRegistrations { get; } = new List<InProgressDomainRegistration>();
}