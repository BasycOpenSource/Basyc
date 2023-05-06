namespace Basyc.Repositories;

public interface IBulkCrudRepository<TModel, TKey> : IBulkRepository, IAsyncReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     If id is null it will be genereted.
    /// </summary>
    TModel Add(TModel model);

    /// <summary>
    ///     Updates a record.
    /// </summary>
    void Update(TModel model);

    /// <summary>
    ///     Deletes item with same id.
    /// </summary>
    void Remove(TKey id);
}
