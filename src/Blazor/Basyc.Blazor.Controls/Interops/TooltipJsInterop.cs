using Basyc.Blazor.Controls.Tooltip;
using Microsoft.JSInterop;

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

	public async void HideTooltip(DotNetObjectReference<TooltipPopup> tooltipElementComponenet, string elementToMoveId, string targetElementId)
	{
		var module = await moduleTask.Value;
		await jsRuntime.InvokeVoidAsync("hideTooltip", tooltipElementComponenet, elementToMoveId, targetElementId);
	}

	public async void ShowTooltip(DotNetObjectReference<TooltipPopup> tooltipElementComponenet, string elementToMoveId, string targetElementQuerySelector = "basycControls")
	{
		var module = await moduleTask.Value;
		await jsRuntime.InvokeVoidAsync("showTooltip", tooltipElementComponenet, elementToMoveId, targetElementQuerySelector);
	}
}
