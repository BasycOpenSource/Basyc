using System.Reflection;

namespace Basyc.Serialization.ProtobufNet;

public record PreparedTypeMetadata(bool HasZeroProperties, PropertyInfo[] PublicProperties);
