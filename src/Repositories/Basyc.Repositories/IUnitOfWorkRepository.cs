using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories;

/// <summary>
/// Repositories used inside unit of work should not have save/commit/complete public implementation
/// </summary>
public interface IUnitOfWorkRepository<TModel, TKey>
{
	/// <summary>
	/// Is responsible for saving repository changes. Indicates unique storage source that can be shared between multiple repositories (e.g. DbContext)
	/// </summary>
	IUnitOfWorkContext Context { get; }
}
