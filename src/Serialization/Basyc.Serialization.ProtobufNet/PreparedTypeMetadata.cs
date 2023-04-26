using System.Reflection;

namespace Basyc.Serialization.ProtobufNet;

#pragma warning disable CA1819 // Properties should not return arrays
public record PreparedTypeMetadata(bool HasZeroProperties, PropertyInfo[] PublicProperties);
