using Basyc.Serailization.SystemTextJson;
using Basyc.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasycSerializerSelectSerializersStageProtobufNetExtensions
{
    public static JsonByteSerializer SystemTextJson(this SerializersSelectSerializerStage basycSerializers) => JsonByteSerializer.Singlenton;
}
