using Nuke.Common.Tooling;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[TypeConverter(typeof(TypeConverter<Configuration>))]
[SuppressMessage("Usage", "CA2211")]
[SuppressMessage("Design", "CA1050")]
public class Configuration : Enumeration
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration) => configuration.Value;
}
