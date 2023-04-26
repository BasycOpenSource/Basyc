namespace Basyc.MessageBus.HttpProxy.Client.Http;

#pragma warning disable CA1056 // URI-like properties should not be strings

public class SignalROptions
{
    public string? SignalRServerUri { get; set; }

    public string? ProxyClientHubPattern { get; set; }
}
