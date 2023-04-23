using Basyc.Serialization.Abstraction;
using System.Text.Json;

namespace Basyc.Serailization.SystemTextJson;

public class JsonByteSerializer : ITypedByteSerializer
{
    public static JsonByteSerializer Singlenton = new JsonByteSerializer();

    public object? Deserialize(byte[] serializedObject, Type dataType) => JsonSerializer.Deserialize(serializedObject, dataType);

    public byte[] Serialize(object? deserializedObject, Type dataType)
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, deserializedObject, dataType);
        return stream.ToArray();
    }

    //public OneOf<object, SerializationFailure> Deserialize(byte[] objectData, Type objectType)
    //{
    //    try
    //    {
    //        return JsonSerializer.Deserialize(objectData, objectType)!;
    //    }
    //    catch (Exception ex)
    //    {
    //        return new SerializationFailure(ex.Message);
    //    }
    //}

    //public OneOf<T, SerializationFailure> DeserializeT<T>(byte[] objectData)
    //{
    //    var result = Deserialize(objectData, typeof(T));
    //    if (result.Value is SerializationFailure)
    //        return result.AsT1;

    //    return (T)result.Value;
    //}

    //public OneOf<string, SerializationFailure> DeserializeToString(string objectData, Type objectType)
    //{
    //    try
    //    {
    //        return JsonSerializer.Serialize(objectData, objectType);
    //    }
    //    catch (Exception ex)
    //    {
    //        return new SerializationFailure(ex.Message);
    //    }
    //}

    //public OneOf<byte[], SerializationFailure> Serialize(object objectData, Type objectType)
    //{
    //    try
    //    {
    //        using var stream = new MemoryStream();
    //        JsonSerializer.Serialize(stream, objectData, objectType);
    //        return stream.ToArray();
    //    }
    //    catch (Exception ex)
    //    {
    //        return new SerializationFailure(ex.Message);
    //    }
    //}

    //public OneOf<byte[], SerializationFailure> SerializeT<T>(T objectData) where T : notnull
    //{
    //    var result = Serialize(objectData, typeof(T));
    //    return result;

    //}

}
