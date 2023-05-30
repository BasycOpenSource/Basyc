using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Controls.Interops;
public partial class ElementJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly ILogger<ElementJsInterop> logger;

    public ElementJsInterop(IJSRuntime jsRuntime, ILogger<ElementJsInterop> logger)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Basyc.Blazor.Controls/elementJSInterop.js").AsTask());
        this.logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async void SetStyle(string elementId, string cssText)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("changeStyleById", elementId, cssText);
    }

    public async void SetStyle(ElementReference elementReference, string cssText)
    {
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("changeStyleByReference", elementReference, cssText);
    }

    public async ValueTask<string> GetCssVariable(ElementReference elementReference, string name)
    {
        logger.LogDebug("GetCssVariable");
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("getCssVariable", elementReference, name);
    }

    public async ValueTask SetCssVariable(ElementReference elementReference, string name, string value)
    {
        //logger.LogDebug("SetCssVariable");
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setCssVariable", elementReference, name, value);
        //logger.LogDebug("SetCssVariable done");
    }

    public async ValueTask SetCssProperty(ElementReference elementReference, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setCssProperty", elementReference, name, value);
    }
}
