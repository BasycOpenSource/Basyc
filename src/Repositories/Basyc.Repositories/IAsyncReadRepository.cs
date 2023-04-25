namespace Basyc.Repositories;

public interface IAsyncReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     Returns all records as dictionary.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Dictionary<TKey, TModel>> GetAllAsync();

    /// <summary>
    ///     Throws exception when not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<TModel?> GetAsync(TKey id);

    /// <summary>
    ///     Returns default when not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<TModel?> TryGetAsync(TKey id);
}
