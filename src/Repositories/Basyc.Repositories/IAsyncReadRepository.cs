namespace Basyc.Repositories;

public interface IAsyncReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     Returns all records as dictionary
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<TKey, TModel>> GetAllAsync();

    /// <summary>
    ///     Throws exception when not found,
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TModel?> GetAsync(TKey id);

    /// <summary>
    ///     Returns default when not found,
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TModel?> TryGetAsync(TKey id);
}
