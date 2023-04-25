using Basyc.Serialization.Abstraction;
using Basyc.Serialization.ProtobufNet;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuildingProtobufSerializationExtensions
{
    public static void SelectProtobufNet(this SelectSerializationStage selectSerializationStage)
    {
        selectSerializationStage.Services.AddSingleton<ITypedByteSerializer, ProtobufByteSerializer>();
        selectSerializationStage.Services.AddSingleton<IObjectToByteSerailizer, ObjectFromTypedByteSerializer>();
    }
}
