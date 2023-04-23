namespace Basyc.Repositories;

public abstract class QueryableTrackingAsyncInstantCrudRepositoryBase<TModel, TKey> : TrackingAsyncInstantCrudRepositoryBase<TModel, TKey>
    where TModel : class where TKey : notnull
{
    protected readonly Func<TModel, TKey> keySelector;
    protected IQueryable<TModel> allRecords;

    public QueryableTrackingAsyncInstantCrudRepositoryBase(IEnumerable<TModel> allRecords,
        Func<TModel, TKey> keySelector) : this(allRecords.AsQueryable(), keySelector)
    {
    }

    public QueryableTrackingAsyncInstantCrudRepositoryBase(IDictionary<TKey, TModel> allRecords,
        Func<TModel, TKey> keySelector) : this(allRecords.Values.AsQueryable(), keySelector)
    {
    }

    public QueryableTrackingAsyncInstantCrudRepositoryBase(IQueryable<TModel> allRecords, Func<TModel, TKey> keySelector)
    {
        this.allRecords = allRecords;
        this.keySelector = keySelector;
    }

    public override Task<TModel?> TryGetAsync(TKey key)
    {
        var model = allRecords.FirstOrDefault(x => keySelector(x).Equals(key));
        return Task.FromResult(model)!;
    }

    public override Task<Dictionary<TKey, TModel>> GetAllAsync()
    {
        var models = allRecords.ToDictionary(keySelector);
        return Task.FromResult(models);
    }
}
