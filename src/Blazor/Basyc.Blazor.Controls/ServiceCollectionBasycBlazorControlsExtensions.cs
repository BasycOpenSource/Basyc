﻿using Basyc.Blazor.Controls.Interops;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionBasycBlazorControlsExtensions
{
	public static IServiceCollection AddBasycBlazorControls(this IServiceCollection services)
	{
		services.AddSingleton<TooltipJsInterop>();
		services.AddSingleton<ScrollJsInterop>();
		return services;
	}
}
