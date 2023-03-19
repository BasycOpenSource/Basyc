using System.Reflection;

namespace Basyc.ReactiveUi;
public static class ReactiveUiTypeExtensions
{
	public static FieldInfo GetReactivePropertyBackingField(this Type type, string propertyName)
	{
		var field = type.GetField($"${propertyName}", BindingFlags.Instance | BindingFlags.NonPublic);
		return field is null
			? throw new InvalidOperationException("Could not find backing field. Did you forget to put [Reactive] attribute on view-model's property?")
			: field;
	}
}
