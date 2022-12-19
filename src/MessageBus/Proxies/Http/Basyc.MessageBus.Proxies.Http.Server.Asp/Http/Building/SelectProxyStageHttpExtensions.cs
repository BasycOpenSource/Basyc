using Basyc.MessageBus.HttpProxy.Server.Asp.Building;
using Basyc.MessageBus.HttpProxy.Server.Asp.Http;
using Basyc.Serialization.Abstraction;
using Basyc.Serialization.ProtobufNet;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SelectProxyStageHttpExtensions
    {
        public static void UseHttp(this SelectProxyStage parent)
        {
            //services.AddSingleton<IRequestSerializer, JsonRequestSerializer>();
            parent.services.AddSingleton<ITypedByteSerializer, ProtobufByteSerializer>();
            parent.services.AddSingleton<IObjectToByteSerailizer, ObjectFromTypedByteSerializer>();
            parent.services.AddSingleton<ProxyHttpRequestHandler>();
        }
    }
}