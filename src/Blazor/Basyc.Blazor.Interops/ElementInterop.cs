using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Interops;
public partial class ElementInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly ILogger<ElementInterop> logger;

    public ElementInterop(IJSRuntime jsRuntime, ILogger<ElementInterop> logger)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Basyc.Blazor.Interops/elementInterop.js").AsTask());
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

    public async ValueTask SetStyle(ElementReference elementReference, string cssText)
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
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setCssVariable", elementReference, name, value);
    }

    public async ValueTask SetCssProperty(ElementReference elementReference, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(elementReference);
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setCssProperty", elementReference, name, value);
    }
}

//[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Source generator.")]
//public partial class ElementJsInterop
//{
//    [JSImport("setStyle", "elementJsInterop")]
//    public static partial void SetStyle(string elementId, string cssText);

//    [JSImport("setStyle", "elementJsInterop")]
//    public static partial void SetStyle([JSMarshalAs<JSType.Any>] object elementReference, string cssText);

//    [JSImport("getCssVariable", "elementJsInterop")]
//    public static partial string GetCssVariable([JSMarshalAs<JSType.Any>] object elementReference, string name);

//    [JSImport("setCssVariable", "elementJsInterop")]
//    public static partial void SetCssVariable([JSMarshalAs<JSType.Any>] object elementReference, string name, string value);

//    [JSImport("setCssProperty", "elementJsInterop")]
//    public static partial void SetCssProperty([JSMarshalAs<JSType.Any>] object elementReference, string name, string value);
//}
