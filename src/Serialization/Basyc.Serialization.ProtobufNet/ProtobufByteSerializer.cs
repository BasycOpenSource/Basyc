using Basyc.Serialization.Abstraction;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Reflection;

namespace Basyc.Serialization.ProtobufNet;

public class ProtobufByteSerializer : ITypedByteSerializer
{
    public static ProtobufByteSerializer Singlenton = new();
    private static readonly Dictionary<Type, PreparedTypeMetadata> knownTypes = new();
    private static PreparedTypeMetadata PrepareSerializer(Type typeToPrepare)
    {
        if (knownTypes.TryGetValue(typeToPrepare, out var metadata))
        {
            return metadata;
        }

        var publicProperties = typeToPrepare.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        bool hasZeroProperties = publicProperties.Length == 0;
        var newMetadata = new PreparedTypeMetadata(hasZeroProperties, publicProperties);
        knownTypes.Add(typeToPrepare, newMetadata);

        bool couldBeSerializedByDefault = RuntimeTypeModel.Default.CanSerialize(typeToPrepare);
        if (couldBeSerializedByDefault is false)
        {
            FixPotentialMissingPropertiesInCtor(typeToPrepare);

            if (RuntimeTypeModel.Default.CanSerialize(typeToPrepare) is false)
            {
                if (TryFixWithSkippingEmptyCtor(typeToPrepare) is false)
                {

                    RuntimeTypeModel.Default.Add(typeToPrepare);
                    if (RuntimeTypeModel.Default.CanSerialize(typeToPrepare) is false)
                    {
                        throw new Exception($"Could not prepare type '{typeToPrepare.Name}'");
                    }
                }
            }
        }

        return newMetadata;
    }

    /// <summary>
    /// Workaround for scenarios when class has empty ctor. Returns false when fix cant be applied.
    /// </summary>
    /// <param name="typeToPrepare"></param>
    /// <returns></returns>
    private static bool TryFixWithSkippingEmptyCtor(Type typeToPrepare)
    {
        if (IsTypeHavingExtraEmptyCtorProblem(typeToPrepare))
        {
            PrepareButSkipCtor(typeToPrepare);
        }
        else
        {
            //Problem could be even nested

            var properties = typeToPrepare.GetProperties();
            foreach (var property in properties)
            {
                bool canSeriProperty = RuntimeTypeModel.Default.CanSerialize(property.PropertyType);
                if (canSeriProperty)
                {
                    continue;
                }

                //PrepareSerializer(property.PropertyType);
                TryFixWithSkippingEmptyCtor(property.PropertyType);
            }
        }

        return RuntimeTypeModel.Default.CanSerialize(typeToPrepare);
    }

    private static void PrepareButSkipCtor(Type typeToPrepare)
    {
        var serializationMetadata = RuntimeTypeModel.Default.Add(typeToPrepare);
        serializationMetadata.UseConstructor = false;
        foreach (var property in typeToPrepare.GetProperties())
        {
            serializationMetadata.Add(property.Name);
        }
    }

    private static bool IsTypeHavingExtraEmptyCtorProblem(Type type)
    {
        var ctors = type.GetConstructors();

        if (ctors.Length <= 1)
        {
            return false;
        }

        //Must contain empty ctor to have the problem
        if (ctors.FirstOrDefault(x => x.GetParameters().Length != 0) is null)
        {
            return false;
        }

        //Must contain ctor with all properties
        if (ctors.Any(x => x.GetParameters().Length == type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Length) is false)
        {
            return false;
        }

        return true;
    }

    private static bool IsTypeMissingPropertiesInCtor(Type type)
    {
        var publicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (publicProperties.Length is 0)
        {
            return false;
        }

        var notEmptyCtors = type.GetConstructors().Where(x => x.GetParameters().Length > 0);
        if (notEmptyCtors.Any(x => x.GetParameters().Length == publicProperties.Length) is false)
        {
            return true;
        }

        return false;
    }

    private static bool FixPotentialMissingPropertiesInCtor(Type type)
    {
        bool hadProblem = false;
        if (IsTypeMissingPropertiesInCtor(type))
        {
            PrepareButSkipCtor(type);
            hadProblem = true;
        }
        else
        {
            //Problem can be nested
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (FixPotentialMissingPropertiesInCtor(property.PropertyType) is true)
                {
                    hadProblem = true;
                }
            }
        }

        return hadProblem;
    }

    public byte[] Serialize(object? input, Type dataType)
    {
        var typeMetadata = PrepareSerializer(dataType);

        if (input == null)
        {
            return new byte[0];
        }

        if (typeMetadata.HasZeroProperties)
        {
            return new byte[0];
        }

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, input);
        return stream.ToArray();
    }

    public object? Deserialize(byte[] input, Type dataType)
    {
        PrepareSerializer(dataType);

        if (input == null)
        {
            return dataType.GetDefaultValue();
        }

        var stream = new MemoryStream(input);
        stream.Write(input, 0, input.Length);
        stream.Seek(0, SeekOrigin.Begin);

        object result = Serializer.Deserialize(dataType, stream);
        return result;
    }

    public bool TrySerialize<T>(T input, out byte[]? output, out SerializationFailure? error)
    {
        var thisCasted = (ITypedByteSerializer)this;
        return thisCasted.TrySerialize(input, typeof(T), out output, out error);
    }

    public bool TryDeserialize<T>(byte[] serializedInput, out T? input, out SerializationFailure? error)
    {
        var thisCasted = (ITypedByteSerializer)this;
        bool wasSuccesful = thisCasted.TryDeserialize(serializedInput, typeof(T), out object? inputObject, out error);
        input = (T?)inputObject;
        return wasSuccesful;

    }

    public byte[] Serialize<T>(object? deserializedObject) => Serialize(deserializedObject, typeof(T));

    public object? Deserialize<T>(byte[] serializedObject) => Deserialize(serializedObject, typeof(T));
}
