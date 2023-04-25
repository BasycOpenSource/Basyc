using System.Diagnostics.CodeAnalysis;

namespace Basyc.Serialization.Abstraction;

public interface ISerializer<TDeserialized, TSerialized, TSerializationMetadata>
{
    public bool TrySerialize(TDeserialized deserializedObject,
        TSerializationMetadata dataType,
        [NotNullWhen(true)] out TSerialized? serializedObject,
        [NotNullWhen(false)] out SerializationFailure? error)
    {
        try
        {
            serializedObject = Serialize(deserializedObject, dataType);
            error = null;
#pragma warning disable CS8762
            return true;
#pragma warning restore CS8762
        }
        catch (Exception ex)
        {
            serializedObject = default;
            error = new SerializationFailure(ex);
            return false;
        }
    }

    bool TryDeserialize(TSerialized serializedObject,
        TSerializationMetadata dataType,
        [NotNullWhen(true)] out TDeserialized? deserializedObject,
        [NotNullWhen(false)] out SerializationFailure? error)
    {
        try
        {
            deserializedObject = Deserialize(serializedObject, dataType);
            error = null;
#pragma warning disable CS8762
            return true;
#pragma warning restore CS8762
        }
        catch (Exception ex)
        {
            deserializedObject = default;
            error = new SerializationFailure(ex);
            return false;
        }
    }

    /// <summary>
    ///     Throws <see cref="SerializationFailureException" /> exception when fails.
    /// </summary>
    TSerialized Serialize(TDeserialized deserializedObject, TSerializationMetadata dataType);

    /// <summary>
    ///     Throws <see cref="SerializationFailureException" /> exception when fails.
    /// </summary>
    TDeserialized Deserialize(TSerialized serializedInput, TSerializationMetadata dataType);
}
