using Basyc.Serailization.SystemTextJson;
using Basyc.Serialization;
using Basyc.Serialization.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasycSerializerSelectSerializersStageProtobufNetExtensions
{
	public static JsonByteSerializer SystemTextJson(this SerializersSelectSerializerStage basycSerializers)
	{
		return JsonByteSerializer.Singlenton;
	}
}
