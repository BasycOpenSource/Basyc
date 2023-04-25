using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Throw;
using Xunit;
//using Kontrer.Shared.Repositories.EF.Tests;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class EfInstantCrudRepositoryTests : IDisposable
{
    private readonly TestDbContext dbContext;

    private readonly DbContextOptions<TestDbContext> dbOptions;

    //private readonly PersonEFCrudRepository peopleRepo;
    //private readonly CarEFCrudRepository carRepo;
    private readonly TestUnitOfWork unitOfWork;

    public EfInstantCrudRepositoryTests()
    {
        dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDbContextInMemoryDb").Options;
        dbContext = new TestDbContext(dbOptions);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        unitOfWork = new TestUnitOfWork(dbContext);
    }

    [Fact]
    public async Task Proxy_Id_Set_Should_Work()
    {
        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Age = new DateTime(2000, 1, 1);
        personToRepo.Name = "John";
        await unitOfWork.People.InstaAddAsync(personToRepo);

        //Need to create a new repo since hidden SetEntityId is not used when entity is cached.
        var newRepo = new TestUnitOfWork(new TestDbContext(dbOptions)).People;

        await newRepo.InstaRemoveAsync(personToRepo.Id);
        (await newRepo.TryGetAsync(personToRepo.Id)).Should().Be(default);
    }

    [Fact]
    public async Task Proxy_Id_Get_Should_Work()
    {
        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Age = new DateTime(2000, 1, 1);
        personToRepo.Name = "John";
        await unitOfWork.People.InstaAddAsync(personToRepo);
        var personFromRepo = await unitOfWork.People.GetAsync(personToRepo.Id);
        personFromRepo.ThrowIfNull();
        personFromRepo.Id.Should().Be(personToRepo.Id);
    }

    [Fact]
    public async Task Should_Update_After_Add_Without_Save()
    {
        var newName = "2";

        var model = new PersonModel();
        model.Id = new Random().Next();
        model.Name = "1";

        await unitOfWork.People.InstaAddAsync(model);
        model.Name = newName;
        await unitOfWork.People.InstaUpdateAsync(model);

        var newModel = await unitOfWork.People.TryGetAsync(model.Id);
        newModel.ThrowIfNull();
        newModel.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Should_Update_After_Add_With_Save()
    {
        var newName = "2";

        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Name = "1";

        await unitOfWork.People.InstaAddAsync(personToRepo);
        personToRepo.Name = newName;
        await unitOfWork.People.InstaUpdateAsync(personToRepo);

        var personFromRepo = await unitOfWork.People.TryGetAsync(personToRepo.Id);
        personFromRepo.ThrowIfNull();
        personFromRepo.Name.Should().Be(newName);
    }

    public void Dispose() => throw new NotImplementedException();
}
