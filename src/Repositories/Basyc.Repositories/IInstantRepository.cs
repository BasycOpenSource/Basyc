namespace Basyc.Repositories;

/// <summary>
/// Performs modyfing actions instatly when called. Repository should not have save/commint/complete implementation
/// <br/>
/// Choose between <see cref="IBulkRepository"/> or <see cref="IInstantRepository"/> Don't use both at the same time!.
/// </summary>
public interface IInstantRepository
{
}
