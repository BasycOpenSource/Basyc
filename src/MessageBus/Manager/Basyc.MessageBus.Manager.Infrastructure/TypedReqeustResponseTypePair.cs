namespace Basyc.MessageBus.Manager.Infrastructure;
#pragma warning disable CA2225 // Operator overloads have named alternates

public record TypedReqeustResponseTypePair(Type RequestType, Type ResponseType)
{
    public static implicit operator TypedReqeustResponseTypePair(Type[] types) => new(types[0], types[1]);
}
