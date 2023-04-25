using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Controls.Interops;
public class ElementJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly IJSRuntime jsRuntime;

    public ElementJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Basyc.Blazor.Controls/elementJSInterop.js").AsTask());
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

    public async void ChangeStyle(string elementId, string cssText)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("changeStyleById", elementId, cssText);
    }

    public async void ChangeStyle(ElementReference elementReference, string cssText)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("changeStyleByReference", elementReference, cssText);
    }
}
