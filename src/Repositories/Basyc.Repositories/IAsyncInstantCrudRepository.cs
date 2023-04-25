namespace Basyc.Repositories;

/// <summary>
///     Does not require Save method, commands are executed immidietly.
/// </summary>
public interface IAsyncInstantCrudRepository<TModel, TKey> : IInstantRepository, IAsyncReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     If id is null it will be generated, After executing properties like Ids are updated.
    /// </summary>
    Task<TModel> InstaAddAsync(TModel model);

    /// <summary>
    ///     Updates record.
    /// </summary>
    Task<TModel> InstaUpdateAsync(TModel model);

    /// <summary>
    ///     Deletes items with same id.
    /// </summary>
    Task InstaRemoveAsync(TKey id);
}
