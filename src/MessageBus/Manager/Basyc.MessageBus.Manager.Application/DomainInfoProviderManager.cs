using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Application
{
	public class DomainInfoProviderManager : IDomainInfoProviderManager
	{
		private readonly IDomainInfoProvider[] messageDomainLoaders;
		private IReadOnlyList<DomainInfo>? domainInfos;


		public DomainInfoProviderManager(IEnumerable<IDomainInfoProvider> messageDomainLoaders)
		{
			this.messageDomainLoaders = messageDomainLoaders.ToArray();
		}

		public IReadOnlyList<DomainInfo> GetDomainInfos()
		{
			domainInfos = messageDomainLoaders.SelectMany(x => x.GenerateDomainInfos()).ToList();
			return domainInfos;
		}
	}
}