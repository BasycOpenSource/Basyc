using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.Repositories;

/// <summary>
/// Repository wich does not contain any navigation properties/other repositories types.
/// Use when this repository is not used with unit of work.
/// <br/>
/// Choose between <see cref="IBulkRepository"/>, <see cref="IAsyncInstantCrudRepository{TModel, TKey}"/> or <see cref="IInstantRepository"/>.
/// </summary>
public interface IBulkRepository
{
	/// <summary>
	/// Saves changes made only with this repository
	/// </summary>
	void Save();

	/// <summary>
	/// Saves changes made only with this repository
	/// </summary>
	Task SaveAsync(CancellationToken cancellationToken = default);
}