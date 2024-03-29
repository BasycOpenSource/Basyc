﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories;

public class RepositoryAction<TModel, TKey>
{
	public RepositoryAction(TKey id, TModel model, string actionType)
	{
		ActionType = actionType;
		NewModelData = model;
		Id = id;
	}

	public string ActionType { get; set; }
	public TModel NewModelData { get; set; }
	public TKey Id { get; set; }
}
