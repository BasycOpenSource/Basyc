using Basyc.Serialization.Abstraction;
using System.Text.Json;

namespace Basyc.Serailization.SystemTextJson;

public class JsonByteSerializer : ITypedByteSerializer
{
    public static readonly JsonByteSerializer Singlenton = new();

    public object? Deserialize(byte[] serializedInput, Type dataType) => JsonSerializer.Deserialize(serializedInput, dataType);

    public byte[] Serialize(object? deserializedObject, Type dataType)
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, deserializedObject, dataType);
        return stream.ToArray();
    }
}
