namespace Basyc.MessageBus.Manager.Infrastructure;

public record TypedReqeustResponseTypePair(Type RequestType, Type ResponseType)
{
    public static implicit operator TypedReqeustResponseTypePair(Type[] types) => new(types[0], types[1]);
}
