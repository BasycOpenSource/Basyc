using System;
using System.Collections.Generic;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure
{
    public class CqrsInterfacedDomainProviderOptions
    {
        public class CQRSRegistration
        {
            public Type IQueryType { get; set; }
            public Type ICommandType { get; set; }
            public Type ICommandWithResponseType { get; set; }
            public Type IMessageType { get; set; }
            public Type IMessageWithResponseType { get; set; }
            public string DomainName { get; set; }

            public IEnumerable<Assembly> AssembliesToScan { get; set; } = new List<Assembly>();

        }

        private readonly List<CQRSRegistration> cQRSRegistrations = new List<CQRSRegistration>();
        public void AddCQRSRegistration(CQRSRegistration cQRSRegistration)
        {
            cQRSRegistrations.Add(cQRSRegistration);
        }

        public List<CQRSRegistration> GetCQRSRegistrations()
        {
            return cQRSRegistrations;
        }
    }

}