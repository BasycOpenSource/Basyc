using System.Diagnostics;
using System.Reflection;

namespace Basyc.ReactiveUi;
public static class ReactiveUiTypeExtensions
{
	public static FieldInfo GetReactivePropertyBackingField(this Type type, string propertyName)
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

		//Looks like reactive ui fody can change the name of backing field to $field.
		var reactiveBackingFieldName = $"${propertyName}";
		var field = type.GetFieldEvenNested(reactiveBackingFieldName, flags);
		// If calling assembly does not call the fix this code can be used as workaround.
		// Might be uncommented later
		//field ??= type.GetFieldEvenNested($"<{propertyName}>k__BackingField", flags);
		if (field is null)
		{
			var allfields = type.GetFields(flags).Select(x => x.Name).ToArray();
			var message = $"Could not find backing field '{reactiveBackingFieldName}' for property '{propertyName}' in type '{type.Name}'. Did you forget to put [Reactive] attribute on view-model's property?. Executing must have reference to ReactiveUI.Fody nuget package. Also try calling {nameof(BasycReactiveUi.Fix)} in executing assembly code (in example App.xml.cs or Program.cs.)";
			Debug.WriteLine(message);
			throw new InvalidOperationException(message);
		}

		return field;
	}
}
