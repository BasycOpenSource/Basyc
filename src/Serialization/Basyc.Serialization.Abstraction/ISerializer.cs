using System.Diagnostics.CodeAnalysis;

namespace Basyc.Serialization.Abstraction;

public interface ISerializer<TDeserialized, TSerialized, TSerializationMetadata>
{
	public bool TrySerialize(TDeserialized deserializedObject, TSerializationMetadata dataType, [NotNullWhen(true)] out TSerialized? serializedObject, [NotNullWhen(false)] out SerializationFailure? error)
	{
		try
		{
			serializedObject = Serialize(deserializedObject, dataType);
			error = null;
			return true;
		}
		catch (Exception ex)
		{
			serializedObject = default;
			error = new SerializationFailure(ex);
			return false;
		}
	}
	bool TryDeserialize(TSerialized serializedObject, TSerializationMetadata dataType, [NotNullWhen(true)] out TDeserialized? deserializedObject, [NotNullWhen(false)] out SerializationFailure? error)
	{
		try
		{
			deserializedObject = Deserialize(serializedObject, dataType);
			error = null;
			return true;
		}
		catch (Exception ex)
		{
			deserializedObject = default;
			error = new SerializationFailure(ex);
			return false;
		}
	}

	/// <summary>
	/// Throws <see cref="SerializationFailureException"/> exception when fails
	/// </summary>
	/// <param name="deserializedObject"></param>
	/// <param name="dataType"></param>
	/// <returns></returns>
	TSerialized Serialize(TDeserialized deserializedObject, TSerializationMetadata dataType);
	/// <summary>
	/// Throws <see cref="SerializationFailureException"/> exception when fails
	/// </summary>
	/// <param name="serializedInput"></param>
	/// <param name="dataType"></param>
	/// <returns></returns>
	TDeserialized Deserialize(TSerialized serializedInput, TSerializationMetadata dataType);
}
