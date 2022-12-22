using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainNameFormatter : ITypedDomainNameFormatter
{
	public string GetFormattedName(Assembly assembly)
	{
		return assembly.GetName().Name;
	}
}