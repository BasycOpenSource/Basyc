namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public static class SignalRConstants
{
    public const string ProxyClientHubPattern = "/proxyClientHub";
    public const string ReceiveRequestResultMetadataMessage = "ReceiveRequestResultMetadata";
    public const string ReceiveRequestResultMessage = "ReceiveRequestResult";
    public const string ReceiveRequestFailedMessage = "ReceiveRequestFailed";
}
