using Basyc.Serializaton.Abstraction;

namespace Basyc.Serialization.Abstraction;

public sealed class ObjectFromTypedByteSerializer : IObjectToByteSerailizer
{
    private readonly ITypedByteSerializer typedByteSerializer;

    public ObjectFromTypedByteSerializer(ITypedByteSerializer typedByteSerializer)
    {
        this.typedByteSerializer = typedByteSerializer;
    }

    public object? Deserialize(byte[] serializedInput, string dataType) => typedByteSerializer.Deserialize(serializedInput, TypedToSimpleConverter.ConvertSimpleToType(dataType));

    public byte[] Serialize(object? deserializedObject, string dataType)
    {
        var clrType = TypedToSimpleConverter.ConvertSimpleToType(dataType);
        var seriResult = typedByteSerializer.Serialize(deserializedObject, clrType);
        return seriResult;
    }
}
