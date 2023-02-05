using ProtoBuf;
using ProtoBuf.Meta;

namespace Basyc.MessageBus.NetMQ.Shared;

public static class TypedObjectToByteSerializer2
{
	static TypedObjectToByteSerializer2()
	{
		Serializer.PrepareSerializer<ProtoMessageWrapper>();
	}

	public static byte[] Serialize(object objectData, Type objectType)
	{
		if (objectData == null)
		{
			return new byte[0];
		}

		if (objectData.GetType().GetProperties().Length == 0)
		{
			return new byte[0];
		}

		using var stream = new MemoryStream();
		PrepareSerializer(objectType);
		Serializer.Serialize(stream, objectData);
		return stream.ToArray();
	}

	private static void PrepareSerializer(Type type)
	{
		if (RuntimeTypeModel.Default.CanSerialize(type) is false)
		{
			RuntimeTypeModel.Default.Add(type);
			var parameters = type.GetConstructors().First().GetParameters();
			foreach (var parameter in parameters)
			{
				PrepareSerializer(parameter.ParameterType);
			}
		}
	}

	public static T? Deserialize<T>(byte[] objectData)
	{
		return (T?)Deserialize(objectData, typeof(T));
	}

	public static object? Deserialize(byte[] objectData, Type objectClrType)
	{
		PrepareSerializer(objectClrType);

		if (objectData == null)
		{
			return objectClrType.GetDefaultValue();
		}

		if (objectData.Length == 0)
		{
			try
			{
				return Activator.CreateInstance(objectClrType)!;
			}
			catch
			{
				throw new Exception("Cannot deserialize message. Message data is empty and message does not have empty constructor.");
			}
		}

		using var stream = new MemoryStream();
		stream.Write(objectData, 0, objectData.Length);
		stream.Seek(0, SeekOrigin.Begin);

		var result = Serializer.Deserialize(objectClrType, stream);
		return result;
	}
}
