using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;

public class BusManagerJSInterop : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> moduleTask;

	public BusManagerJSInterop(IJSRuntime jsRuntime)
	{
		moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
		   "import", "./_content/Basyc.MessageBus.Manager.Presentation.BlazorLibrary/BusManagerJSInterop.js").AsTask());
	}

	public async Task ApplyChangesToIndexHtml()
	{
		var module = await moduleTask.Value;
		await module.InvokeVoidAsync("addBusMangerStaticFiles");
	}

	public async ValueTask DisposeAsync()
	{
		if (moduleTask.IsValueCreated)
		{
			var module = await moduleTask.Value;
			await module.DisposeAsync();
		}
	}
}