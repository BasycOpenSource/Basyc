using Microsoft.JSInterop;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;

public class BusManagerJsInterop : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> moduleTask;

	public BusManagerJsInterop(IJSRuntime jsRuntime)
	{
		moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
			"import", "./_content/Basyc.MessageBus.Manager.Presentation.BlazorLibrary/BusManagerJSInterop.js").AsTask());
	}

	public async ValueTask DisposeAsync()
	{
		if (moduleTask.IsValueCreated)
		{
			var module = await moduleTask.Value;
			await module.DisposeAsync();
		}
	}

	public async Task ApplyChangesToIndexHtml()
	{
		var module = await moduleTask.Value;
		await module.InvokeVoidAsync("addBusMangerStaticFiles");
	}
}
