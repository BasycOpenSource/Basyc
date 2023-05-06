using Nuke.Common.Tooling;
using System.ComponentModel;

#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable CA1050 // Declare types in namespaces
[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration) => configuration.Value;
}
