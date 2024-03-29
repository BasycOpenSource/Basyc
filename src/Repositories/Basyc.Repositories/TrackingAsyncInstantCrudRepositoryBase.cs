﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.Repositories;

public abstract class TrackingAsyncInstantCrudRepositoryBase<TModel, TKey> : IAsyncInstantCrudRepository<TModel, TKey>, ITrackingChangesRepository<TModel, TKey>
	   where TModel : class
{
	public List<RepositoryAction<TModel, TKey>> Actions { get; private set; } = new List<RepositoryAction<TModel, TKey>>();

	public TrackingAsyncInstantCrudRepositoryBase()
	{
	}

	public abstract Task<Dictionary<TKey, TModel>> GetAllAsync();

	public abstract Task<TModel> TryGetAsync(TKey key);

	public abstract Task<TModel> GetAsync(TKey id);

	protected abstract TKey GetModelId(TModel model);

	public Task InstaRemoveAsync(TKey id)
	{
		var oldUpdate = Actions.FirstOrDefault(x => x.Id.Equals(id));
		if (oldUpdate == null)
		{
			Actions.Add(new RepositoryAction<TModel, TKey>(id, null, CrudActions.Removed));
		}
		else
		{
			switch (oldUpdate.ActionType)
			{
				case CrudActions.Added:
					Actions.Remove(oldUpdate);
					break;

				case CrudActions.Modified:
					Actions.Remove(oldUpdate);
					break;

				case CrudActions.Removed:
					break;
			}
		}

		return Task.CompletedTask;
	}

	public Task<TModel> InstaAddAsync(TModel model)
	{
		var id = GetModelId(model);
		var newUpdate = new RepositoryAction<TModel, TKey>(id, model, CrudActions.Added);
		Actions.Add(newUpdate);
		return Task.FromResult(model);
	}

	public Task<TModel> InstaUpdateAsync(TModel model)
	{
		var id = GetModelId(model);
		var newUpdate = new RepositoryAction<TModel, TKey>(id, model, CrudActions.Modified);
		var oldUpdate = Actions.FirstOrDefault(x => x.Id.Equals(id));
		if (oldUpdate == null)
		{
			Actions.Add(newUpdate);
		}
		else
		{
			var index = Actions.IndexOf(oldUpdate);
			newUpdate.ActionType = oldUpdate.ActionType;
			Actions[index] = newUpdate;
		}

		return Task.FromResult(model);
	}
}
