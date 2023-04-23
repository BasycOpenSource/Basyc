using Microsoft.JSInterop;

namespace Basyc.Blazor.Controls.Interops;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.
public class TooltipJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public TooltipJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Basyc.Blazor.Controls/tooltipJSInterop.js").AsTask());
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async void HideTooltip(string elementToMoveId, string targetElementId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("hideTooltip", elementToMoveId, targetElementId);
    }

    public async void ShowTooltip(string elementToMoveId, string targetElementQuerySelector = "basycControls")
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("showTooltip", elementToMoveId, targetElementQuerySelector);
    }
}
