using Basyc.Serialization;
using Basyc.Serialization.Abstraction;
using Basyc.Serialization.ProtobufNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasycSerializerSelectSerializersStageProtobufNetExtensions
{
	public static ProtobufByteSerializer ProtobufNet(this SerializersSelectSerializerStage basycSerializers)
	{
		return ProtobufByteSerializer.Singlenton;
	}
}
