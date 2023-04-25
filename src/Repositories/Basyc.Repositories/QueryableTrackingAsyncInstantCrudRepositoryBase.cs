﻿namespace Basyc.Repositories;

public abstract class QueryableTrackingAsyncInstantCrudRepositoryBase<TModel, TKey> : TrackingAsyncInstantCrudRepositoryBase<TModel, TKey>
    where TModel : class where TKey : notnull
{
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
        this.AllRecords = allRecords;
        this.KeySelector = keySelector;
    }

    protected Func<TModel, TKey> KeySelector { get; init; }

    protected IQueryable<TModel> AllRecords { get; private set; }

    public override Task<TModel?> TryGetAsync(TKey id)
    {
        var model = AllRecords.FirstOrDefault(x => KeySelector(x).Equals(id));
        return Task.FromResult(model)!;
    }

    public override Task<Dictionary<TKey, TModel>> GetAllAsync()
    {
        var models = AllRecords.ToDictionary(KeySelector);
        return Task.FromResult(models);
    }
}
