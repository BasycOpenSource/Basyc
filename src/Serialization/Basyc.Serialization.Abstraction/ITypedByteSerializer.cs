namespace Basyc.Serialization.Abstraction
{
	public interface ITypedByteSerializer : ISerializer<object?, byte[], Type>
	{

		public bool TrySerialize<T>(T deserializedObject, out byte[]? serializedObject, out SerializationFailure? error)
		{
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
		}
		public bool TryDeserialize<T>(byte[] serializedObject, out T? deserializedObject, out SerializationFailure? error)
		{
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
		}

		public byte[] Serialize<T>(object? deserializedObject)
		{
			return Serialize(deserializedObject, typeof(T));
		}

		public object? Deserialize<T>(byte[] serializedObject)
		{
			return Deserialize(serializedObject, typeof(T));
		}

	}
}
