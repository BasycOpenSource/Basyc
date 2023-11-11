using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Interops;

public abstract class InteropBase : IAsyncDisposable
{
    private readonly IJSRuntime jsRuntime;

    private readonly string jsModuleUrl;

    private IJSObjectReference? moduleTask;

    protected InteropBase(IJSRuntime jsRuntime, ILogger<InteropBase> logger, Uri jsModuleUrl)
        : this(jsRuntime, logger, jsModuleUrl.OriginalString)
    {
    }

    protected InteropBase(IJSRuntime jsRuntime, ILogger<InteropBase> logger, string jsModuleUrl)
    {
        this.jsRuntime = jsRuntime;
        Logger = logger;
        this.jsModuleUrl = jsModuleUrl;
    }

    protected ILogger<InteropBase> Logger { get; }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask is not null)
            await moduleTask.DisposeAsync();
    }

    protected async ValueTask<IJSObjectReference> GetJsModule()
    {
        moduleTask ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", jsModuleUrl);
        return moduleTask;
    }
}
