namespace Basyc.Serialization;

/// <summary>
/// If you install any Basyc serializer nuget package they will be visible here (Added with extensio methods).
/// </summary>
public static class BasycSerializers
{
    public static SerializersSelectSerializerStage Select() => new();
}
