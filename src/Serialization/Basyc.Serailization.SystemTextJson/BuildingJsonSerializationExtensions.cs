using Basyc.Serailization.SystemTextJson;
using Basyc.Serialization.Abstraction;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuildingJsonSerializationExtensions
{
    public static void SelectSytemTextJson(this SelectSerializationStage selectSerializationStage)
    {
        selectSerializationStage.Services.AddSingleton<ITypedByteSerializer, JsonByteSerializer>();
        selectSerializationStage.Services.AddSingleton<IObjectToByteSerailizer, ObjectFromTypedByteSerializer>();
    }
}
