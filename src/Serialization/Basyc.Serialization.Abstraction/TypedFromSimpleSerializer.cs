using Basyc.Serializaton.Abstraction;

namespace Basyc.Serialization.Abstraction;

public sealed class TypedFromSimpleSerializer : ITypedByteSerializer
{
    private readonly IObjectToByteSerailizer byteSerailizer;

    public TypedFromSimpleSerializer(IObjectToByteSerailizer byteSerailizer)
    {
        this.byteSerailizer = byteSerailizer;
    }

    public object? Deserialize(byte[] serializedInput, Type dataType) => byteSerailizer.Deserialize(serializedInput, TypedToSimpleConverter.ConvertTypeToSimple(dataType));

    public byte[] Serialize(object? deserializedObject, Type dataType) => byteSerailizer.Serialize(deserializedObject, TypedToSimpleConverter.ConvertTypeToSimple(dataType));
}
