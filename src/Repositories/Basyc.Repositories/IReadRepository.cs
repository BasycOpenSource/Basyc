namespace Basyc.Repositories;

public interface IReadRepository<TModel, TKey> where TKey : notnull
{
    /// <summary>
    ///     Returns all records as dictionary.
    /// </summary>
    Dictionary<TKey, TModel> GetAll();

    /// <summary>
    ///     Throws exception when not found.
    /// </summary>
    TModel Get(TKey id);

    /// <summary>
    ///     Returns default when not found.
    /// </summary>
    TModel TryGet(TKey id);
}
