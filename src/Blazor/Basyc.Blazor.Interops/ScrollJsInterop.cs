using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Basyc.Blazor.Interops;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.
//public class ScrollJsInterop : IAsyncDisposable
//{
//    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

//    public ScrollJsInterop(IJSRuntime jsRuntime)
//    {
//        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
//            "import", "./_content/Basyc.Blazor.Controls/scrollJSInterop.js").AsTask());
//    }

//    public async ValueTask DisposeAsync()
//    {
//        if (moduleTask.IsValueCreated)
//        {
//            var module = await moduleTask.Value;
//            await module.DisposeAsync();
//        }
//    }

//    public async void AddDragToScroll(string elementId)
//    {
//        var module = await moduleTask.Value;
//        await module.InvokeVoidAsync("addDragToScroll", elementId);
//    }
//}

[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Source generator.")]
public partial class ScrollJsInterop
{
    [JSImport("addDragToScroll", "scrollJsInterop")]
    public static partial string AddDragToScroll(string elementId);
}