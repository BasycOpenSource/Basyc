using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface
{
	public class InterfaceDomainProviderOptions
	{
		public List<InterfaceRegistration> InterfaceRegistrations { get; } = new();
	}

	public class InterfaceRegistration
	{
		public const string DefaultRequester = "DefaultRequester";
		public List<Assembly> AssembliesToScan { get; set; }
		public string DomainName { get; set; }
		public Type MessageInterfaceType { get; set; }
		public bool HasResponse { get; set; }
		public Type ResponseType { get; set; }
		public Func<Type, string> DisplayNameFormatter { get; set; }
		public string ResponseDisplayName { get; set; }
		public RequestType RequestType { get; set; }
		public string RequesterUniqueName { get; set; }

	}
}
