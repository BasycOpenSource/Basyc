﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basyc.Repositories;

public interface IReadRepository<TModel, TKey>
{
	/// <summary>
	/// Returns all records as dictionary
	/// </summary>
	/// <returns></returns>
	Dictionary<TKey, TModel> GetAll();

	/// <summary>
	/// Throws exception when not found,
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	TModel Get(TKey id);

	/// <summary>
	/// Returns default when not found,
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	TModel TryGet(TKey id);
}
