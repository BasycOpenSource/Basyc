using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using System;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class InProgressMessageRegistration
{
	public string MessagDisplayName { get; set; }
	public RequestType MessageType { get; set; }
	public List<ParameterInfo> Parameters { get; } = new List<ParameterInfo>();
	public Action<RequestContext> RequestHandler { get; set; }
	public Type ResponseRunTimeType { get; set; }
	public string ResponseRunTimeTypeDisplayName { get; set; }
	public bool HasResponse => ResponseRunTimeType is not null;

}
