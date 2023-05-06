using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class PersonEfCrudRepository : EfInstantCrudRepositoryBase<PersonEntity, int, PersonModel>
{
    public PersonEfCrudRepository(DbContext dbContext, ILogger<PersonEfCrudRepository> logger) : base(dbContext, x => x.Id, x => x.Id, logger)
    {
    }

    protected override PersonEntity? ToEntity(PersonModel model)
    {
        if (model == null)
        {
            return null;
        }

        var entity = new PersonEntity();
        entity.Id = model.Id;
        entity.Age = model.Age;
        entity.Name = model.Name;

        return entity;
    }

    protected override PersonModel? ToModel(PersonEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var model = new PersonModel();
        model.Id = entity.Id;
        model.Age = entity.Age;
        model.Name = entity.Name;

        return model;
    }
}
