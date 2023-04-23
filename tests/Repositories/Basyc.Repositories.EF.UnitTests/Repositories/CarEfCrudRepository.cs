using Microsoft.Extensions.Logging;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class CarEfCrudRepository : EfInstantCrudRepositoryBase<CarEntity, int, CarModel>
{
    public CarEfCrudRepository(TestDbContext dbContext, ILogger<CarEfCrudRepository> logger) : base(dbContext, car => car.Id, car => car.Id, logger)
    {
    }

    protected override CarEntity? ToEntity(CarModel model)
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

    protected override CarModel? ToModel(CarEntity entity)
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
