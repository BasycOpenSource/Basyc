using Basyc.Serializaton.Abstraction;

namespace Basyc.Serialization.Abstraction;

public sealed class ObjectFromTypedByteSerializer : IObjectToByteSerailizer
{
	private readonly ITypedByteSerializer typedByteSerializer;

	public ObjectFromTypedByteSerializer(ITypedByteSerializer typedByteSerializer)
	{
		this.typedByteSerializer = typedByteSerializer;
	}

	public object? Deserialize(byte[] serializedInput, string dataType)
	{
		return typedByteSerializer.Deserialize(serializedInput, TypedToSimpleConverter.ConvertSimpleToType(dataType));
	}

	public byte[] Serialize(object? input, string dataType)
	{
		var clrType = TypedToSimpleConverter.ConvertSimpleToType(dataType);
		var seriResult = typedByteSerializer.Serialize(input, clrType);
		return seriResult;
	}

	public bool TryDeserialize(byte[] serializedInput, string dataType, out object? input, out SerializationFailure? error)
	{
		return typedByteSerializer.TryDeserialize(serializedInput, TypedToSimpleConverter.ConvertSimpleToType(dataType), out input, out error);
	}

	public bool TrySerialize(object? input, string dataType, out byte[]? output, out SerializationFailure? error)
	{
		return typedByteSerializer.TrySerialize(input, TypedToSimpleConverter.ConvertSimpleToType(dataType), out output, out error);

	}
}