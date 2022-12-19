using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers
{
	public static class ReturnObjectHelper
	{
		public static void CheckHandlerReturnType(object returnObject, Type expectedType)
		{
			if (returnObject is null)
			{
				bool cannotBeNull = expectedType.IsValueType || Nullable.GetUnderlyingType(expectedType) == null;
				if (cannotBeNull is false)
				{
					throw new InvalidOperationException($"Handler return null but expected type is {expectedType} does not support null");
				}
				return;
			}

			var returnObjectType = returnObject.GetType();
			if (returnObjectType.IsAssignableTo(expectedType) is false)
			{
				throw new InvalidOperationException($"Handler return object of type {returnObject.GetType()} but expected type is {expectedType}");
			}
		}
	}
}
