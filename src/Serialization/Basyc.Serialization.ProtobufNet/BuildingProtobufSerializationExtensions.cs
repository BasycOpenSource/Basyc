using Basyc.Serialization.Abstraction;
using Basyc.Serialization.ProtobufNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BuildingProtobufSerializationExtensions
    {
        public static void SelectProtobufNet(this SelectSerializationStage selectSerializationStage)
        {
            selectSerializationStage.services.AddSingleton<ITypedByteSerializer, ProtobufByteSerializer>();
            selectSerializationStage.services.AddSingleton<IObjectToByteSerailizer, ObjectFromTypedByteSerializer>();
        }
    }
}
