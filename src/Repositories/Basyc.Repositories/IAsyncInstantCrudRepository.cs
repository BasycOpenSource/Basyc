namespace Basyc.Repositories;

/// <summary>
///     Does not require Save method, commands are executed immidietly
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IAsyncInstantCrudRepository<TModel, TKey> : IInstantRepository, IAsyncReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     If id is null it will be genereted, After executing properties like Ids are updated
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    Task<TModel> InstaAddAsync(TModel model);

    /// <summary>
    ///     Updates record
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    Task<TModel> InstaUpdateAsync(TModel model);

    /// <summary>
    ///     Deletes items with same id.
    /// </summary>
    /// <param name="id"></param>
    Task InstaRemoveAsync(TKey id);
}
