namespace System
{
	public static class GenericsHelper
	{
		public static bool IsAssignableToGenericType(this Type childType, Type parentType)
		{
			var interfaceTypes = childType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == parentType)
					return true;
			}

			if (childType.IsGenericType && childType.GetGenericTypeDefinition() == parentType)
				return true;

			Type baseType = childType.BaseType;
			if (baseType == null)
				return false;

			return IsAssignableToGenericType(baseType, parentType);
		}

		public static bool IsAssignableToGenericType<TChildType>(Type parentType)
		{
			var childType = typeof(TChildType);
			return IsAssignableToGenericType(childType, parentType);
		}

		public static bool IsAssignableToGenericType<TChildType, TParent>()
		{
			var parentType = typeof(TParent);
			var childType = typeof(TChildType);
			return IsAssignableToGenericType(childType, parentType);
		}

		/// <summary>
		/// Get generic argument from base class
		/// </summary>
		/// <param name="childType"></param>
		/// <param name="parentType">parent type that should contain generic parameters</param>
		/// <returns></returns>

		public static Type[] GetTypeArgumentsFromParent(this Type childType, Type parentType)
		{
			if (parentType.IsGenericTypeDefinition is false)
				parentType = parentType.GetGenericTypeDefinition();

			if (childType.IsGenericType)
			{
				if (childType.GetGenericTypeDefinition() == parentType)
				{
					return childType.GetGenericArguments();
				}
			}

			if (parentType.IsInterface)
			{
				var baseInterface = childType.GetInterface(parentType.Name);
				if (baseInterface is null)
				{
					throw new InvalidOperationException("Class does not have specified base class/interface");
				}
				return baseInterface.GetGenericArguments();
			}

			while (childType.BaseType != null)
			{
				childType = childType.BaseType;
				if (childType.IsGenericType && childType.GetGenericTypeDefinition() == parentType)
				{
					return childType.GetGenericArguments();
				}
			}
			if (childType != typeof(object))
			{
				throw new InvalidOperationException("Class does not have specified base class/interface");
			}
			else
			{
				return new Type[0];
			}
		}
	}
}