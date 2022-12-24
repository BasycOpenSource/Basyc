using Basyc.Serailization.SystemTextJson;
using Basyc.Serialization.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuildingJsonSerializationExtensions
{
	public static void SelectSytemTextJson(this SelectSerializationStage selectSerializationStage)
	{
		selectSerializationStage.services.AddSingleton<ITypedByteSerializer, JsonByteSerializer>();
		selectSerializationStage.services.AddSingleton<IObjectToByteSerailizer, ObjectFromTypedByteSerializer>();
	}
}
