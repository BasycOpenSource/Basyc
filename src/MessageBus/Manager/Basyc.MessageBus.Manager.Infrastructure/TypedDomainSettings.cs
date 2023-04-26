namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainSettings
{
    /// <summary>
    ///     Name of request group, usually name of a service e.g CustomerService.
    /// </summary>
    public string? DomainName { get; set; }

    /// <summary>
    ///     If you dont know waht CQRS is, use only <see cref="GenericRequestTypes" /> and <see cref="GenericRequestWithResponseTypes" /> properties.
    /// </summary>
    public ICollection<Type> GenericRequestTypes { get; init; } = new List<Type>();

    /// <summary>
    ///     If you dont know waht CQRS is, use only <see cref="GenericRequestTypes" /> and <see cref="GenericRequestWithResponseTypes" /> properties.
    /// </summary>
    public ICollection<TypedReqeustResponseTypePair> GenericRequestWithResponseTypes { get; init; } = new List<TypedReqeustResponseTypePair>();

    /// <summary>
    ///     Use when your system implements CQRS.
    /// </summary>
    public ICollection<TypedReqeustResponseTypePair> QueryTypes { get; init; } = new List<TypedReqeustResponseTypePair>();

    /// <summary>
    ///     Use when your system implements CQRS.
    /// </summary>
    public ICollection<Type> CommandTypes { get; init; } = new List<Type>();

    /// <summary>
    ///     Use when your system implements CQRS.
    /// </summary>
    public ICollection<TypedReqeustResponseTypePair> CommandWithResponseTypes { get; init; } = new List<TypedReqeustResponseTypePair>();
}
