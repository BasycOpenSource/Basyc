namespace Basyc.Serialization.Abstraction;

public interface ITypedByteSerializer : ISerializer<object?, byte[], Type>
{
    public bool TrySerialize<T>(T deserializedObject, out byte[]? serializedObject, out SerializationFailure? error)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            serializedObject = Serialize(deserializedObject, typeof(T));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            serializedObject = default;
            error = new SerializationFailure(ex);
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public bool TryDeserialize<T>(byte[] serializedObject, out T? deserializedObject, out SerializationFailure? error)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            deserializedObject = (T?)Deserialize(serializedObject, typeof(T));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            deserializedObject = default;
            error = new SerializationFailure(ex);
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public byte[] Serialize<T>(object? deserializedObject) => Serialize(deserializedObject, typeof(T));

    public object? Deserialize<T>(byte[] serializedObject) => Deserialize(serializedObject, typeof(T));
}
