using Microsoft.JSInterop;
using System.Xml.Linq;

namespace Basyc.Blazor.Controls.Interops;
public class TooltipJsInterop : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> moduleTask;
	private readonly IJSRuntime jsRuntime;

	public TooltipJsInterop(IJSRuntime jsRuntime)
	{
		moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
			"import", "./_content/Basyc.Blazor.Controls/tooltipJSInterop.js").AsTask());
		this.jsRuntime = jsRuntime;
	}

	public async ValueTask DisposeAsync()
	{
		if (moduleTask.IsValueCreated)
		{
			var module = await moduleTask.Value;
			await module.DisposeAsync();
		}
	}

	//public async void HideTooltip(string elementToMoveId, string targetElementId)
	//{
	//	var module = await moduleTask.Value;
	//	await module.InvokeVoidAsync("hideTooltip", elementToMoveId, targetElementId);
	//}

	//public async void ShowTooltip(string elementToMoveId, string targetElementQuerySelector = "basycControls")
	//{
	//	var module = await moduleTask.Value;
	//	await module.InvokeVoidAsync("showTooltip", elementToMoveId, targetElementQuerySelector);
	//}

	public async void HideTooltip(DotNetObjectReference<BasycTooltip> tooltipElementComponenet, string elementToMoveId, string targetElementId)
	{
		var module = await moduleTask.Value;
		//await module.InvokeVoidAsync("hideTooltip", objRef, elementToMoveId, targetElementId);
		//await module.InvokeVoidAsync("hideTooltip", objRef);
		//await jsRuntime.InvokeVoidAsync("hideTooltip", objRef);
		await jsRuntime.InvokeVoidAsync("hideTooltip", tooltipElementComponenet, elementToMoveId, targetElementId);
	}

	public async void ShowTooltip(DotNetObjectReference<BasycTooltip> tooltipElementComponenet, string elementToMoveId, string targetElementQuerySelector = "basycControls")
	{
		var module = await moduleTask.Value;
		//await module.InvokeVoidAsync("showTooltip", objRef, elementToMoveId, targetElementQuerySelector);
		//await module.InvokeVoidAsync("showTooltip", objRef);
		//await jsRuntime.InvokeVoidAsync("showTooltip", objRef);
		await jsRuntime.InvokeVoidAsync("showTooltip", tooltipElementComponenet, elementToMoveId, targetElementQuerySelector);
	}
}
