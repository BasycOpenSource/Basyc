using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedRequestNameFormatter : ITypedRequestNameFormatter
{
	public string GetFormattedName(Type requestType)
	{
		var requestName = requestType.Name
			.Replace("Command", string.Empty, StringComparison.OrdinalIgnoreCase)
			.Replace("Request", string.Empty, StringComparison.OrdinalIgnoreCase)
			.Replace("Message", string.Empty, StringComparison.OrdinalIgnoreCase)
			.Replace("Query", string.Empty, StringComparison.OrdinalIgnoreCase);

		return requestName;
	}
}