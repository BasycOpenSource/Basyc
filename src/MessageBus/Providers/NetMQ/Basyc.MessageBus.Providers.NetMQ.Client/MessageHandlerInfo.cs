using System.Reflection;

namespace Basyc.MessageBus.Client.NetMQ;

public class NetMqMessageHandlerInfo
{
    public NetMqMessageHandlerInfo(string messageSimpleType, /* Type handlerType,*/ Type messageType, MethodInfo handleMethod)
    {
        MessageSimpleType = messageSimpleType;
        //HandlerType = handlerType;
        MessageType = messageType;
        HandleMethodInfo = handleMethod;
    }

    public NetMqMessageHandlerInfo(string messageSimpleType, /*Type handlerType,*/ Type messageType, Type responseType, MethodInfo handleMethod)
        : this(messageSimpleType, /*handlerType,*/ messageType, handleMethod)
    {
        ResponseType = responseType;
        HasResponse = true;
    }

    public Type MessageType { get; }
    public Type? ResponseType { get; }
    public bool HasResponse { get; }
    public MethodInfo HandleMethodInfo { get; }

    public string MessageSimpleType { get; }
    //public Type HandlerType { get; }
}
