﻿using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class InProgressMessageRegistration
{
	public string? MessageDisplayName { get; set; }
	public MessageType MessageType { get; set; }
	public List<ParameterInfo> Parameters { get; } = new();
	public Action<MessageRequest>? RequestHandler { get; set; }
	public Type? ResponseRunTimeType { get; set; }
	public string? ResponseRunTimeTypeDisplayName { get; set; }
	public bool HasResponse => ResponseRunTimeType is not null;
}
