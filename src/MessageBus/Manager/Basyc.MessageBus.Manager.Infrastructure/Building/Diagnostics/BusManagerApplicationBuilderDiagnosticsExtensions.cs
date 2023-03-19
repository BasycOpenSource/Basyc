﻿using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;

public static class BusManagerApplicationBuilderDiagnosticsExtensions
{
	public static SetupDiagnosticsStage EnableDiagnostics(this BusManagerApplicationBuilder parent)
	{
		return new SetupDiagnosticsStage(parent.services);
	}
}