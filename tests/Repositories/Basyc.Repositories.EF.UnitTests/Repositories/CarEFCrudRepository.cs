using Basyc.Repositories.EF;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class CarEFCrudRepository : EFInstantCrudRepositoryBase<CarEntity, int, CarModel>
{
	public CarEFCrudRepository(TestDbContext dbContext, ILogger<CarEFCrudRepository> logger) : base(dbContext, car => car.Id, car => car.Id, logger)
	{
	}

	protected override CarEntity ToEntity(CarModel model)
	{
		if (model == null)
		{
			return null;
		}

		var entity = new CarEntity();
		entity.Age = model.Age;
		entity.Id = model.Id;
		entity.Name = model.Name;
		entity.CustomerId = model.CustomerId;
		return entity;
	}

	protected override CarModel ToModel(CarEntity entity)
	{
		if (entity == null)
		{
			return null;
		}

		var model = new CarModel();
		model.Age = entity.Age;
		model.Id = entity.Id;
		model.Name = entity.Name;
		model.CustomerId = entity.CustomerId;
		return model;
	}
}