using Basyc.Serialization;
using Basyc.Serialization.ProtobufNet;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasycSerializerSelectSerializersStageProtobufNetExtensions
{
    public static ProtobufByteSerializer ProtobufNet(this SerializersSelectSerializerStage basycSerializers) => ProtobufByteSerializer.Singlenton;
}
