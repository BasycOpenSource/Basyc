using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager;

public class TypedDddDomainNameFormatter : ITypedDomainNameFormatter
{
    public string GetFormattedName(Assembly assembly)
    {
        var customName = assembly.GetName().Name.Split('.')[^2];
        return customName;
    }
}
