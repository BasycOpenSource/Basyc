using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.Repositories
{
    public abstract class QueryableTrackingAsyncInstantCrudRepositoryBase<TModel, TKey> : TrackingAsyncInstantCrudRepositoryBase<TModel, TKey>
       where TModel : class
    {
        protected IQueryable<TModel> allRecords;
        protected readonly Func<TModel, TKey> keySelector;

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

        public override Task<TModel> TryGetAsync(TKey key)
        {
            var model = allRecords.FirstOrDefault(x => keySelector(x).Equals(key));
            return Task.FromResult(model);
        }

        public override Task<Dictionary<TKey, TModel>> GetAllAsync()
        {
            Dictionary<TKey, TModel> models = allRecords.ToDictionary(keySelector);
            return Task.FromResult(models);
        }
    }
}