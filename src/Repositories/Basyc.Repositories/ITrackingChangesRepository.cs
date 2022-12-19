using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.Repositories
{
    /// <summary>
    /// Tracking changes in collection used for later execution (Unit of work). Don't use with <see cref="IInstantRepository"/>
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ITrackingChangesRepository<TModel, TKey>
    {
        List<RepositoryAction<TModel, TKey>> Actions { get; }
    }
}