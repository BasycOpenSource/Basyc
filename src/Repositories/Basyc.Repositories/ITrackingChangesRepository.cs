namespace Basyc.Repositories;

#pragma warning disable CA1002 // Do not expose generic lists

/// <summary>
/// Tracking changes in collection used for later execution (Unit of work). Don't use with <see cref="IInstantRepository"/>.
/// </summary>
public interface ITrackingChangesRepository<TModel, TKey>
{
    List<RepositoryAction<TModel, TKey>> Actions { get; }
}
